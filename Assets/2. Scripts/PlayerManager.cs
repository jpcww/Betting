using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    #region Player
    public GameObject playerInstance;
    private PhotonView photonView;
    #endregion

    #region UI
    PlayerUI playerUI;
    #endregion

    #region Event
    public Dealer dealer;
    #endregion

    #region Network
    private ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    #endregion

    #region Bet
    public int selectedColor;
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
    public Transform table;
    private List<GameObject> ownedChips = new List<GameObject>();
    private List<GameObject> betChips = new List<GameObject>();
    private List<GameObject> removedOwnedChips = new List<GameObject>();
    private List<GameObject> removedBetChips = new List<GameObject>();

    private Vector3 coinPosition = new Vector3(0, 0.1f, 0);
    private Vector3 xVector = new Vector3(0.1f, 0, 0);
    private Vector3 zVector = new Vector3(0, 0, 0.1f);
    #endregion

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        InitiatePlayer();
    }

    
    private void OnEnable()
    {
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

    private void OnDisable()
    {
        if(photonView.IsMine)
            playerInput.Disable();
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

    /*
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("room properties");
        if (propertiesThatChanged.ContainsKey("revealedColor"))
        {
            ProcessWinLose((int)propertiesThatChanged["revealedColor"]);
            
            Debug.Log("revealed color in the property : " + (int)propertiesThatChanged["revealedColor"]);
        }
    }
    */

    private void InitiatePlayer()
    {
        // PlAYER
        if(photonView.IsMine)
        {
            // Dealer
            dealer = PhotonView.Find(1).gameObject.GetComponent<Dealer>();

            int seamNumber = photonView.ViewID % 3;
            playerInstance = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
            playerInstance.transform.SetParent(PlayerSpawner.playerSpawner.seats[seamNumber].transform);
            playerInstance.transform.localPosition = Vector3.zero;
            playerInstance.transform.LookAt(dealer.transform);
            playerInstance.GetComponentInChildren<Camera>().enabled = true;
            playerInstance.GetComponentInChildren<Canvas>().enabled = true;

            // CHIPS
            //table = playerInstance.transform.parent.parent;
            //InstantiateChips();

            // UI
            playerUI = playerInstance.GetComponentInChildren<PlayerUI>();
            playerUI.SetUserName(photonView.Owner.NickName);
            playerUI.SetColor(Color.gray);

            // Network Communication
            playerProperties.Add("selectedColor", -1);
            playerProperties.Add("betAmount", betAmount);
            playerProperties.Add("hasBet", hasBet);
            playerProperties.Add("hasReceivedColor", false);
            playerProperties.Add("hasProcessedWinLose", false);

            StartCoroutine(WaitForRevealedColor());
        }
    }


    public IEnumerator WaitForRevealedColor()
    {
        yield return new WaitUntil(() => playerProperties.ContainsKey("hasReceivedColor") && (bool)playerProperties["hasReceivedColor"] == false && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("hasRevealedColor") && (bool)PhotonNetwork.CurrentRoom.CustomProperties["hasRevealedColor"] == true);

        playerProperties["hasReceivedColor"] = true;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);

        ProcessWinLose((int)PhotonNetwork.CurrentRoom.CustomProperties["revealedColor"]);
    }

    // SELECT COLORS
    private void SelectGreen()
    {
        if (!selectGreenInput)
            return;

        if (!hasBet)
        {
            selectedColor = (int)Colors.green;
            playerUI.SetColor(Color.green);

            // Network communication
            playerProperties["selectedColor"] = (int)Colors.green;

            hasSelectedColor = true;
            selectGreenInput = false;
        }
    }
    private void SelectRed()
    {
        if (!selectRedInput)
            return;

        if (!hasBet)
        {
            selectedColor = (int)Colors.red;
            playerUI.SetColor(Color.red);

            // Network communication
            playerProperties["selectedColor"] = (int)Colors.red;

            hasSelectedColor = true;
            selectRedInput = false;
        }
    }

    // DECIDE AMOUNT OF BETTING
    public void DecideBetAmout()
    {
        if (!decideBetAmountInput)
            return;

        if(!hasSelectedColor)
        {
            playerUI.DisplayWarningMessage("Please Select Color");
            decideBetAmountInput = false;
            return;
        }

        if (!hasBet && betAmount < currentChipAmount)
        {
            betAmount += defaultBetAmount;
            playerUI.DisplayBetAmount(string.Format("Bet Amount : ${0}", betAmount));

            // Network communication
            playerProperties["betAmount"] = betAmount;

            decideBetAmountInput = false;
            return;
        }

        else if(!hasBet && betAmount >= currentChipAmount)
        {
            playerUI.DisplayWarningMessage("Not Enough Chips");
            decideBetAmountInput = false;
            return;
        }

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

        if (!hasSelectedColor)
        {
            playerUI.DisplayWarningMessage("Please Select Color");
            betInput = false;
            return;
        }

        if (hasSelectedColor)
        {
            // UI
            currentChipAmount -= betAmount;
            playerUI.DisplayerCurrentChipAmount(string.Format("Current Chip : ${0}", currentChipAmount));

            // Move Chips
            BetChips(betAmount);

            // Initialize UI
            hasBet = true;
            betInput = false;

            // Network communication
            playerProperties["hasBet"] = hasBet;
            PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        }
    }

    // PROCESS WIN/LOSE
    private void ProcessWinLose(int revealedColor)
    {
        Debug.Log("ProcessWinLose");
        if(revealedColor == selectedColor)
        {
            EarnChips();
        }

        else
        {
            LoseChips();
        }

        hasSelectedColor = false;
        hasBet = false;
        playerProperties["hasBet"] = hasBet;
        playerProperties["hasReceivedColor"] = false;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);  // It seems to take time to update through the server

        // Reinitialize Coroutine in Dealer
        StartCoroutine(dealer.WaitUntilAllBet());
        StartCoroutine(WaitForRevealedColor());
    }

    // EARN CHIPS
    public void EarnChips()
    {
        // UI
        currentChipAmount += betAmount;
        playerUI.DisplayWinLose(true);
        playerUI.DisplayerCurrentChipAmount(string.Format("Current Chip : ${0}", currentChipAmount));

        // CHIPS
        GetChipsBack(betAmount);

        // Reinitialize Flags
        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }
    // LOSE CHIPS
    public void LoseChips()
    {
        playerUI.DisplayWinLose(false);
        playerUI.DisplayerCurrentChipAmount(string.Format("Current Chip : ${0}", currentChipAmount));

        // CHIPS
        RemoveChips(betAmount);

        if (currentChipAmount <= 0)
        {
            currentChipAmount += 100;

            RefillChips();
        }

        // Reinitialize Flags
        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }

    public void InstantiateChips()
    {
        Color color = new Color();

        for (int i = 0; i < 10; i++)
        {
            // MY CHIPS
            // Instantiate
            GameObject myChipStack = PhotonNetwork.InstantiateRoomObject("ChipStack", Vector3.zero, Quaternion.identity);
            myChipStack.transform.SetParent(table.GetChild(1));
            PhotonView photonView_chipStack = myChipStack.GetPhotonView();
            photonView_chipStack.TransferOwnership(PhotonNetwork.LocalPlayer);

            // Position
            if (i < 5)
                myChipStack.transform.localPosition = coinPosition - xVector * i;
            else
                myChipStack.transform.localPosition = coinPosition - zVector - xVector * (i - 5);

            // Color
            int colorNumber = UnityEngine.Random.Range(0, 11);
            Renderer myChipRenderer = myChipStack.GetComponent<Renderer>();
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

            myChipRenderer.material.color = color;

            ownedChips.Add(myChipStack);

            // BET CHIPS
            // Instantiate
            GameObject chipStack = PhotonNetwork.InstantiateRoomObject("ChipStack", Vector3.zero, Quaternion.identity);
            chipStack.transform.SetParent(table.GetChild(2));

            // Position
            if (i < 5)
                chipStack.transform.localPosition = coinPosition - xVector * i;
            else
                chipStack.transform.localPosition = coinPosition - zVector - xVector * (i - 5);

            Renderer betChipRenderer = myChipStack.GetComponent<Renderer>();
            betChipRenderer.material.color = color;

            chipStack.SetActive(false);
            betChips.Add(chipStack);
        }
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

    private void RefillChips()
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
}
