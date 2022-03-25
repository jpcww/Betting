using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region Player
    string userID;
    #endregion

    #region UI
    PlayerUI playerUI;
    #endregion

    #region Bet
    public Dealer dealer;
    [SerializeField]
    private Color selectedColor;
    int selectedColorNumber = -1;
    [SerializeField]
    private int revealedColorNumber;
    private bool hasSelectedColor = false;
    public int currentChipAmount = 100;
    private int defaultBetAmount = 10;
    public int betAmount = 0;
    public bool hasBet = false;
    #endregion

    #region Input
    PlayerInput playerInput;
    bool selectGreenInput = false;
    bool selectRedInput = false;
    bool decideBetAmountInput = false;
    bool betInput = false;
    #endregion

    #region Coin
    public Transform transForm_availableChips;
    public Transform transForm_betChips;
    public List<GameObject> ownedChips = new List<GameObject>();
    public List<GameObject> betChips = new List<GameObject>();
    public List<GameObject> removedOwnedChips = new List<GameObject>();
    public List<GameObject> removedBetChips = new List<GameObject>();
    #endregion

    private void Start()
    {
        if(photonView.IsMine)
            photonView.RPC("OnPlayerSpawned", RpcTarget.All);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Input
            SelectGreen();
            SelectRed();
            DecideBetAmout();
            Bet();
        }
    }

    public override void OnEnable()
    {
        // Photon Event
        PhotonNetwork.AddCallbackTarget(this);

        // Input
        if (photonView.IsMine)
        {
            if (playerInput == null)
            {
                playerInput = new PlayerInput();

                playerInput.Bet.SelectGreen.performed += i => selectGreenInput = true;
                playerInput.Bet.SelectRed.performed += i => selectRedInput = true; ;
                playerInput.Bet.DecideBetAmount.performed += i => decideBetAmountInput = true;
                playerInput.Bet.Bet.started += i => betInput = true;
            }

            playerInput.Enable();
        }
    }

    public override void OnDisable()
    {
        // Photon Event
        PhotonNetwork.RemoveCallbackTarget(this);

        // Input
        if (photonView.IsMine)
            playerInput.Disable();
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        // Event : Color Announced
        if(eventCode == Dealer.ColorAnnounceEventCode)
        {
            revealedColorNumber = (int)photonEvent.CustomData;
            ProcessWinLose(revealedColorNumber);
        }
    }

    [PunRPC]
    // PLAYER SPAWN
    private void OnPlayerSpawned()
    {
        // Player : Remote
        // Dealer
        dealer = PhotonView.Find(1).gameObject.GetComponent<Dealer>();

        // Seat the player
        int seatNumber = photonView.ViewID % 3 + 1;
        GameObject seat = GameObject.FindGameObjectWithTag(string.Format($"Seat_{seatNumber}"));
        transform.SetParent(seat.transform);
        transform.localPosition = Vector3.zero;
        transform.LookAt(dealer.transform);

        // Player : Local Only
        // UI
        if (photonView.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<Canvas>().enabled = true;

            playerUI = GetComponentInChildren<PlayerUI>();
            playerUI.SetUserName(photonView.Owner.NickName);
            playerUI.SetColor(Color.gray);

            // Photon Event
            PlayerSpawnEvent();

            InitiateChips();
        }
    }

    private void PlayerSpawnEvent()
    {
        userID = PhotonNetwork.LocalPlayer.UserId;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Dealer.PlayerSpawnedEventCode, userID, raiseEventOptions, SendOptions.SendReliable);
    }

    // SELECT COLORS    
    private void SelectGreen()
    {
        if (!photonView.IsMine)
            return;
        if (!selectGreenInput)
            return;

        if (!hasBet)
        {
            // Local UI
            selectedColor = Color.green;
            playerUI.SetColor(selectedColor);

            // Flags
            hasSelectedColor = true;
            selectGreenInput = false;
        }
    }
    private void SelectRed()
    {
        if (!photonView.IsMine)
            return;
        if (!selectRedInput)
            return;

        if (!hasBet)
        {
            // Local UI
            selectedColor = Color.red;
            playerUI.SetColor(selectedColor);

            // Flags
            hasSelectedColor = true;
            selectGreenInput = false;
        }
    }

    // DECIDE AMOUNT OF BETTING
    public void DecideBetAmout()
    {
        if (!(decideBetAmountInput && photonView.IsMine))
            return;

        // Not selected color yet
        if (!hasSelectedColor)
        {
            playerUI.DisplayWarningMessage("Please Select Color");
            decideBetAmountInput = false;
            return;
        }
        
        // Avaialble to bet
        if (!hasBet && betAmount < currentChipAmount)
        {
            betAmount += defaultBetAmount;
            playerUI.DisplayBetAmount(string.Format($"Bet Amount : ${betAmount}"));
            decideBetAmountInput = false;
            return;
        }

        // Not enough chips
        else if(!hasBet && betAmount >= currentChipAmount)
        {
            playerUI.DisplayWarningMessage("Not Enough Chips");
            decideBetAmountInput = false;
            return;
        }

        // Already bet
        else if(hasBet)
        {
            playerUI.DisplayWarningMessage("Already Bet");
            decideBetAmountInput = false;
            return;
        }
    }

    // BET
    public void Bet()
    {
        if (!betInput|| hasBet)
            return;
        if (!photonView.IsMine)
            return;

        // Not yet selected color
        if (!hasSelectedColor)
        {
            playerUI.DisplayWarningMessage("Please Select Color");
            betInput = false;
            return;
        }

        // Bet
        if (hasSelectedColor)
        {
            // UI
            currentChipAmount -= betAmount;
            playerUI.DisplayerCurrentChipAmount(string.Format($"Current Chip : ${currentChipAmount}"));

            // Move Chips
            BetChips(betAmount);

            // Initialize UI
            hasBet = true;
            betInput = false;

            // Photon Event
            BetEvent();
        }
    }

    private void BetEvent()
    {
        if (selectedColor == Color.green)
            selectedColorNumber = 0;
        else if (selectedColor == Color.red)
            selectedColorNumber = 1;

        
        object[] playerInfo = new object[] { userID, selectedColorNumber, betAmount };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(Dealer.BetEventCode, playerInfo, raiseEventOptions, SendOptions.SendReliable);
    }

    // PROCESS WIN/LOSE
    private void ProcessWinLose(int revealedColorNumber)
    {
        if(revealedColorNumber == selectedColorNumber)
        {
            photonView.RPC("EarnChips", RpcTarget.All);
        }

        else
        {
            photonView.RPC("LoseChips", RpcTarget.All);
        }

        hasSelectedColor = false;
        hasBet = false;
    }

    // EARN CHIPS
    [PunRPC]
    public void EarnChips()
    {
        if (photonView.IsMine)
        {
            // UI
            currentChipAmount += betAmount;
            playerUI.DisplayWinLose(true);
            playerUI.DisplayerCurrentChipAmount(string.Format($"Current Chip : ${currentChipAmount}"));
            playerUI.DisplayBetAmount(string.Format($"Bet Amount : ${betAmount}"));

            // Reinitialize Flags
            betAmount = 0;
            hasBet = false;
            hasSelectedColor = false;
            selectedColor = Color.gray;
        }

        // CHIPS
        GetChipsBack(betAmount);

        // TODO : RESTART COROUTINE OF WAITUNTILALLBET() IN DEALER
    }
    // LOSE CHIPS
    [PunRPC]
    public void LoseChips()
    {
        if (photonView.IsMine)
        {
            playerUI.DisplayWinLose(false);
            playerUI.DisplayerCurrentChipAmount(string.Format($"Current Chip : ${currentChipAmount}"));
            playerUI.DisplayBetAmount(string.Format($"Bet Amount : ${betAmount}"));

            // Reinitialize Flags
            betAmount = 0;
            hasBet = false;
            hasSelectedColor = false;
            selectedColor = Color.gray;
        }

        // CHIPS
        RemoveChips(betAmount);

        if (currentChipAmount <= 0)
        {
            currentChipAmount += 100;

            RefillChips();
        }
        // TODO : RESTART COROUTINE OF WAITUNTILALLBET() IN DEALER
    }

    #region Methods : for Chips
    public void InitiateChips()
    {
        // Placement of chips
        Transform transForm_availableChips = transform.GetChild(2);
        Transform transForm_betChips = transform.GetChild(3);

        for (int i = 0; i < 10; i++)
        {
            // Color
            int colorNumber = UnityEngine.Random.Range(0, 11);

            photonView.RPC("InstantiateChip", RpcTarget.AllBuffered, i, colorNumber);
        }
    }

    [PunRPC]
    private void InstantiateChip(int index, int colorNumber)
    {
        Color color = new Color();

        switch (colorNumber)
        {
            case 0:
                color = Color.green;
                break;
            case 1:
                color = Color.red;
                break;
            case 2:
                color = Color.blue;
                break;
            case 3:
                color = Color.black;
                break;
            case 4:
                color = Color.cyan;
                break;
            case 5:
                color = Color.white;
                break;
            case 6:
                color = Color.gray;
                break;
            case 7:
                color = Color.yellow;
                break;
            case 8:
                color = Color.magenta;
                break;
            case 9:
                color = Color.HSVToRGB(63, 183, 209);
                break;
        }

        // Instantiate
        GameObject ownedChipStack = PhotonNetwork.Instantiate("ChipStack", Vector3.zero, Quaternion.identity, 0, null);
        Chip ownedChip = ownedChipStack.GetComponent<Chip>();

        ownedChip.photonView_player = photonView;
        ownedChip.parentObject = transForm_availableChips;
        ownedChip.chipNumber = index;
        ownedChip.color = color;

        ownedChips.Add(ownedChipStack);

        GameObject betChipStack = PhotonNetwork.Instantiate("ChipStack", Vector3.zero, Quaternion.identity, 0, null);
        Chip betChip = betChipStack.GetComponent<Chip>();

        betChip.photonView_player = photonView;
        betChip.parentObject = transForm_betChips;
        betChip.chipNumber = index;
        betChip.color = color;
        betChip.GetComponent<MeshRenderer>().enabled = false;

        betChips.Add(betChipStack);
    }

    private void BetChips(int betAmount)
    {
        for (int i = 0; i < betAmount / 10; i++)
        {
            ownedChips[i].SetActive(false);
            betChips[i].SetActive(true);
        }
    }

    private void GetChipsBack(int betAmount)
    {
        for (int i = 0; i < betAmount / 10; i++)
        {
            ownedChips[i].SetActive(true);
            betChips[i].SetActive(false);
        }
    }

    private void RemoveChips(int betAmount)
    {
        int range = betAmount / 10;
        removedOwnedChips.AddRange(ownedChips.GetRange(0, range));
        for (int i = 0; i < range; i++)
            removedOwnedChips[i].SetActive(false);
        ownedChips.RemoveRange(0, range);

        removedBetChips.AddRange(betChips.GetRange(0, range));
        for (int i = 0; i < range; i++)
            removedBetChips[i].SetActive(false);
        betChips.RemoveRange(0, range);
    }

    private void RefillChips() // TODO : IMPLEMENT COROUTINE TO WAIT TO GIVE TIME TO PLAYERS
    {
        List<GameObject> temp = ownedChips;
        ownedChips.Clear();
        ownedChips.AddRange(removedOwnedChips);
        ownedChips.AddRange(temp);
        foreach (GameObject ownedChip in ownedChips)
            ownedChip.SetActive(true);

        temp.Clear();

        temp = betChips;
        betChips.Clear();
        betChips.AddRange(removedBetChips);
        betChips.AddRange(temp);
        foreach (GameObject betChip in betChips)
            betChip.SetActive(false);
    }
    #endregion

    // TODO : Destroy chips after a use leaves
}
