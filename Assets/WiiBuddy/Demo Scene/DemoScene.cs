using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DemoScene : MonoBehaviour {
	public Transform nunchuk;
	public Transform wiimote;
	public static int whichRemote;
	public static int totalRemotes = 0;
	
	
	public GameObject buttonC;
	public GameObject buttonZ;
	public Transform nunchuckStick;
	
	public GameObject buttonA;
	public GameObject buttonB;
	
	public GameObject buttonUp;
	public GameObject buttonDown;
	public GameObject buttonLeft;
	public GameObject buttonRight;
	
	public GameObject buttonMinus;
	public GameObject buttonPlus;
	public GameObject buttonHome;
	
	public GameObject buttonOne;
	public GameObject buttonTwo;
	
	
	public GameObject buttonClassicA;
	public GameObject buttonClassicB;
	public GameObject buttonClassicMinus;
	public GameObject buttonClassicPlus;
	public GameObject buttonClassicHome;
	public GameObject buttonClassicX;
	public GameObject buttonClassicY;
	public GameObject buttonClassicUp;
	public GameObject buttonClassicDown;
	public GameObject buttonClassicLeft;
	public GameObject buttonClassicRight;
	
	public Transform classic;
	public Transform classicStickLeft;
	public Transform classicStickRight;
	
	public GameObject buttonClassicL;
	public GameObject buttonClassicR;
	
	public GameObject buttonClassicZL;
	public GameObject buttonClassicZR;
	
	public GameObject buttonGuitarGreen;
	public GameObject buttonGuitarRed;
	public GameObject buttonGuitarYellow;
	public GameObject buttonGuitarBlue;
	public GameObject buttonGuitarOrange;
	public GameObject buttonGuitarPlus;
	public GameObject buttonGuitarMinus;
	public Transform guitarBody;
	public Transform guitarNeck;
	public Transform guitarStick;
	public Transform guitarStrum;
	public Transform guitarWhammy;
	
	public Transform drumSet;
	public MeshRenderer drumMinus;
	public MeshRenderer drumPlus;
	public Transform drumStick;
	public Transform drumGreen;
	public Transform drumBlue;
	public Transform drumRed;
	public Transform drumOrange;
	public Transform drumYellow;
	
	public Transform hub;
	public Transform dial;
	public Transform slider;
	public Transform tableStick;
	public GameObject buttonTablePlus;
	public GameObject buttonTableMinus;
	public GameObject butttonEuphoria;
	public GameObject turnTableLeft;
	public GameObject turnTableRight;
	public Transform platterLeft;
	public Transform platterRight;
	public GameObject tableLeftGreen;
	public GameObject tableLeftRed;
	public GameObject tableLeftBlue;
	public GameObject tableRightGreen;
	public GameObject tableRightRed;
	public GameObject tableRightBlue;
	
	
	public Transform motionPlus;
	
	public RectTransform theIRMain;
	public RectTransform theIR1;
	public RectTransform theIR2;
	public RectTransform theIR3;
	public RectTransform theIR4;
	
	public Transform balanceBoard;
	public Transform balanceTopLeft;
	public Transform balanceTopRight;
	public Transform balanceBottomLeft;
	public Transform balanceBottomRight;


	public GameObject buttonProUA;
	public GameObject buttonProUB;
	public GameObject buttonProUMinus;
	public GameObject buttonProUPlus;
	public GameObject buttonProUHome;
	public GameObject buttonProUX;
	public GameObject buttonProUY;
	public GameObject buttonProUUp;
	public GameObject buttonProUDown;
	public GameObject buttonProULeft;
	public GameObject buttonProURight;
	
	public Transform proU;
	public Transform proUStickLeft;
	public Transform proUStickRight;
	
	public GameObject buttonProUL;
	public GameObject buttonProUR;
	
	public GameObject buttonProUZL;
	public GameObject buttonProUZR;
	
	
	//private var searching = false;
	public Text theText;
	public Text theRemoteNumber;

	public GameObject searchButton;
	public GameObject cancelButton;
	public RectTransform selectbuttonsPanel;
	public GameObject selectButtonPrefab;
	private Button[] selectButtons;
	public GameObject dropButton;
	public GameObject detectMotionButton;
	public GameObject deactivateMotionButton;
	public GameObject calibrateMotionButton;
	public GameObject uncalibrateMotionButton;
	public GameObject calibrateWiiUProButton;
	// Use this for initialization
	void OnEnable() {
		theText.text = "Ready.";
		wiimote.gameObject.SetActive (false);
		nunchuk.gameObject.SetActive (false);
		classic.gameObject.SetActive (false);
		guitarBody.gameObject.SetActive (false);
		motionPlus.gameObject.SetActive (false);
		balanceBoard.gameObject.SetActive (false);
		hub.gameObject.SetActive (false);
		drumSet.gameObject.SetActive (false);
		proU.gameObject.SetActive (false);

		selectButtons = new Button[16];

		for(int x = 0; x<16;x++)
		{
			selectButtons[x] = Instantiate(selectButtonPrefab).GetComponent<Button>();
			selectButtons[x].gameObject.name = x+"";
			selectButtons[x].transform.SetParent(selectbuttonsPanel);
			//selectButtons[x].interactable=false;
			selectButtons[x].GetComponentInChildren<Text>().text = "#"+x;
			selectButtons[x].transform.localScale = Vector3.one;
		}

		Destroy(selectButtonPrefab);
		Wii.OnDiscoveryFailed     += OnDiscoveryFailed;
		Wii.OnWiimoteDiscovered   += OnWiimoteDiscovered;
		Wii.OnWiimoteDisconnected += OnWiimoteDisconnected;
	}

	void OnDisable()
	{
		Wii.OnDiscoveryFailed     -= OnDiscoveryFailed;
		Wii.OnWiimoteDiscovered   -= OnWiimoteDiscovered;
		Wii.OnWiimoteDisconnected -= OnWiimoteDisconnected;
	}
	
	// Update is called once per frame
	void Update () {
		totalRemotes = Wii.GetRemoteCount();

		theRemoteNumber.text = "Remote on Display:"+(whichRemote+1).ToString();

		for(var x=0;x<16;x++)
		{
			if(Wii.IsActive(x))
			{
				//remote button visible or not?
				selectButtons[x].interactable = true;
			}
			else
			{
				selectButtons[x].interactable = false;
			}
		}

		if (Wii.IsActive(whichRemote))
		{
			dropButton.SetActive(true);

			if(Wii.HasMotionPlus(whichRemote))
			{
				//deactivate motion plus
				deactivateMotionButton.SetActive(true);
				detectMotionButton.SetActive(false);

				if(Wii.IsMotionPlusCalibrated(whichRemote))
				{
					Debug.Log ("motion plus is calibrated");
					//uncalibrate motion plus
					uncalibrateMotionButton.SetActive(true);
					calibrateMotionButton.SetActive(false);
				}
				else
				{
					Debug.Log ("motion plus is uncalibrated");
					uncalibrateMotionButton.SetActive(false);
					calibrateMotionButton.SetActive(true);
					//calibrate motion plus
				}   
			}
			else
			{
				calibrateMotionButton.SetActive(false);
				uncalibrateMotionButton.SetActive(false);
				deactivateMotionButton.SetActive(false);
				detectMotionButton.SetActive(true);
				//check for motion plus
			}


			theRemoteNumber.enabled=true;
			var inputDisplay = "";
			inputDisplay = inputDisplay + "Remote #"+whichRemote.ToString();
			inputDisplay = inputDisplay + "\nbattery "+Wii.GetBattery(whichRemote).ToString();
			
			if(Wii.GetExpType(whichRemote)==3)//balance board is in is in
			{
				balanceBoard.gameObject.SetActive(true);
				wiimote.gameObject.SetActive(false);

				Vector4 theBalanceBoard = Wii.GetBalanceBoard(whichRemote); 
				Vector2 theCenter = Wii.GetCenterOfBalance(whichRemote);
				//Debug.Log(theBalanceBoard+" "+theCenter);
				balanceTopLeft.localScale     = new Vector3(balanceTopLeft.localScale.x,1f-(.01f*theBalanceBoard.y),balanceTopLeft.localScale.z); 
				balanceTopRight.localScale    = new Vector3(balanceTopRight.localScale.x,1f-(.01f*theBalanceBoard.x),balanceTopRight.localScale.z);
				balanceBottomLeft.localScale  = new Vector3(balanceBottomLeft.localScale.x,1f-(.01f*theBalanceBoard.w),balanceBottomLeft.localScale.z);
				balanceBottomRight.localScale = new Vector3(balanceBottomRight.localScale.x,1f-(.01f*theBalanceBoard.z),balanceBottomRight.localScale.z);

				theIR1.position = new Vector2(Screen.width/2-(Screen.width/4),Screen.height/2+(Screen.height/4));
				theIR1.sizeDelta = new Vector2(10,10);

				theIR2.position = new Vector2(Screen.width/2+(Screen.width/4),Screen.height/2+(Screen.height/4));
				theIR2.sizeDelta = new Vector2(10,10);

				theIR3.position = new Vector2(Screen.width/2-(Screen.width/4),Screen.height/2-(Screen.height/4));
				theIR3.sizeDelta = new Vector2(10,10);

				theIR4.position = new Vector2(Screen.width/2+(Screen.width/4),Screen.height/2-(Screen.height/4));
				theIR4.sizeDelta = new Vector2(10,10);

				theIRMain.position = new Vector2((Screen.width/2)+(theCenter.x*(Screen.width/4)),(Screen.height/2)+(theCenter.y*Screen.height/4));
				theIRMain.sizeDelta = new Vector2(50,50);
				
				inputDisplay = inputDisplay + "\nBALANCE BOARD";
				inputDisplay = inputDisplay + "\ntotal Weight "+Wii.GetTotalWeight(whichRemote)+"kg";
				inputDisplay = inputDisplay + "\ntopRight     "+theBalanceBoard.x+"kg";
				inputDisplay = inputDisplay + "\ntopLeft      "+theBalanceBoard.y+"kg";
				inputDisplay = inputDisplay + "\nbottomRight  "+theBalanceBoard.z+"kg";
				inputDisplay = inputDisplay + "\nbottomLeft   "+theBalanceBoard.w+"kg";
			}
			else
			{
				///WIIREMOTE
				wiimote.gameObject.SetActive(true);
				Vector3[] pointerArray = Wii.GetRawIRData(whichRemote);		
				Vector2 mainPointer = Wii.GetIRPosition(whichRemote);
				Vector3 wiiAccel = Wii.GetWiimoteAcceleration(whichRemote);
				
				theIRMain.position = new Vector2(mainPointer.x*Screen.width,mainPointer.y*Screen.height);
				theIRMain.sizeDelta = new Vector2(50,50);
				float sizeScale = 5.0f;

				theIRMain.position = new Vector2(mainPointer.x*Screen.width,mainPointer.y*Screen.height);
				theIRMain.sizeDelta = new Vector2(50,50);

				theIR1.position = new Vector2(pointerArray[0].x*Screen.width-(pointerArray[0].z*sizeScale/2.0f),
				                              pointerArray[0].y*Screen.height-(pointerArray[0].z*sizeScale/2.0f));
				theIR1.sizeDelta = new Vector2(pointerArray[0].z*sizeScale*10,pointerArray[0].z*sizeScale*10);

				theIR2.position = new Vector2(pointerArray[1].x*Screen.width-(pointerArray[1].z*sizeScale/2.0f),
				                              pointerArray[1].y*Screen.height-(pointerArray[1].z*sizeScale/2.0f));
				theIR2.sizeDelta = new Vector2(pointerArray[1].z*sizeScale*10,pointerArray[1].z*sizeScale*10);

				theIR3.position = new Vector2(pointerArray[2].x*Screen.width-(pointerArray[2].z*sizeScale/2.0f),
				                              pointerArray[2].y*Screen.height-(pointerArray[2].z*sizeScale/2.0f));
				theIR3.sizeDelta = new Vector2(pointerArray[2].z*sizeScale*10,pointerArray[2].z*sizeScale*10);

				theIR4.position = new Vector2(pointerArray[3].x*Screen.width-(pointerArray[3].z*sizeScale/2.0f),
				                              pointerArray[3].y*Screen.height-(pointerArray[3].z*sizeScale/2.0f));
				theIR4.sizeDelta = new Vector2(pointerArray[3].z*sizeScale*10,pointerArray[3].z*sizeScale*10);
				
				wiimote.localRotation = Quaternion.Slerp(transform.localRotation,
				                                         Quaternion.Euler(wiiAccel.y*90.0f, 0.0f,wiiAccel.x*-90.0f),5.0f);
				
				if(Wii.GetButton(whichRemote, "A"))
					buttonA.SetActive (true);
				else
					buttonA.SetActive (false);
				if(Wii.GetButton(whichRemote, "B"))
					buttonB.SetActive (true);
				else
					buttonB.SetActive (false);
				if(Wii.GetButton(whichRemote, "UP"))
					buttonUp.SetActive (true);
				else
					buttonUp.SetActive (false);
				if(Wii.GetButton(whichRemote, "DOWN"))
					buttonDown.SetActive (true);
				else
					buttonDown.SetActive (false);
				if(Wii.GetButton(whichRemote, "LEFT"))
					buttonLeft.SetActive (true);
				else
					buttonLeft.SetActive ( false);
				if(Wii.GetButton(whichRemote, "RIGHT"))
					buttonRight.SetActive (true);
				else
					buttonRight.SetActive (false);
				if(Wii.GetButton(whichRemote, "MINUS"))
					buttonMinus.SetActive (true);
				else
					buttonMinus.SetActive (false);
				if(Wii.GetButton(whichRemote, "PLUS"))
					buttonPlus.SetActive (true);
				else
					buttonPlus.SetActive (false);
				if(Wii.GetButton(whichRemote, "HOME"))
					buttonHome.SetActive (true);
				else
					buttonHome.SetActive (false);
				if(Wii.GetButton(whichRemote, "ONE"))
					buttonOne.SetActive (true);
				else
					buttonOne.SetActive (false);		
				if(Wii.GetButton(whichRemote, "TWO"))
					buttonTwo.SetActive (true);
				else
					buttonTwo.SetActive (false);
				
				inputDisplay = inputDisplay + "\nIR      "+Wii.GetIRPosition(whichRemote).ToString("#.0000");
				inputDisplay = inputDisplay + "\nIR rot  "+Wii.GetIRRotation(whichRemote).ToString();
				inputDisplay = inputDisplay + "\nA       "+Wii.GetButton(whichRemote,"A").ToString();
				inputDisplay = inputDisplay + "\nB       "+Wii.GetButton(whichRemote,"B").ToString();
				inputDisplay = inputDisplay + "\n1       "+Wii.GetButton(whichRemote,"1").ToString();
				inputDisplay = inputDisplay + "\n2       "+Wii.GetButton(whichRemote,"2").ToString();
				inputDisplay = inputDisplay + "\nUp      "+Wii.GetButton(whichRemote,"UP").ToString();
				inputDisplay = inputDisplay + "\nDown    "+Wii.GetButton(whichRemote,"DOWN").ToString();
				inputDisplay = inputDisplay + "\nLeft    "+Wii.GetButton(whichRemote,"LEFT").ToString();
				inputDisplay = inputDisplay + "\nRight   "+Wii.GetButton(whichRemote,"RIGHT").ToString();
				inputDisplay = inputDisplay + "\n-       "+Wii.GetButton(whichRemote,"-").ToString();
				inputDisplay = inputDisplay + "\n+       "+Wii.GetButton(whichRemote,"+").ToString();
				inputDisplay = inputDisplay + "\nHome    "+Wii.GetButton(whichRemote,"HOME").ToString();
				inputDisplay = inputDisplay + "\nAccel   "+Wii.GetWiimoteAcceleration(whichRemote).ToString("#.0000");
				
				if(Wii.HasMotionPlus(whichRemote))
				{
					motionPlus.gameObject.SetActive (true);
					Vector3 motion = Wii.GetMotionPlus(whichRemote);
					if(Input.GetKeyDown("space") || Wii.GetButtonDown(whichRemote,"HOME"))
					{
						motionPlus.localRotation = Quaternion.identity;
					}
					motionPlus.RotateAround(motionPlus.position,motionPlus.right,motion.x);
					motionPlus.RotateAround(motionPlus.position,motionPlus.up,-motion.y);
					motionPlus.RotateAround(motionPlus.position,motionPlus.forward,motion.z);
					
					inputDisplay = inputDisplay + "\nMotion+ "+motion.ToString("#.0000");
					inputDisplay = inputDisplay + "\nYAW FAST "+Wii.IsYawFast(whichRemote);
					inputDisplay = inputDisplay + "\nROLL FAST "+Wii.IsRollFast(whichRemote);
					inputDisplay = inputDisplay + "\nPITCH FAST "+Wii.IsPitchFast(whichRemote);
					
					
					
				}
				else
				{
					motionPlus.gameObject.SetActive(false);
				}
				
				if(Wii.GetExpType(whichRemote)==1)//nunchuck is in
				{
					nunchuk.gameObject.SetActive(true);
					nunchuk.localRotation = Quaternion.Slerp(transform.localRotation,
					                                         Quaternion.Euler(Wii.GetNunchuckAcceleration(whichRemote).y * 90.0f,
					                 0.0f, 
					                 Wii.GetNunchuckAcceleration(whichRemote).x*-90f),
					                                         5.0f);
					
					nunchuckStick.rotation = nunchuk.rotation;
					nunchuckStick.RotateAround(nunchuckStick.position,nunchuckStick.right,Wii.GetAnalogStick(whichRemote).y*30.0f);
					nunchuckStick.RotateAround(nunchuckStick.position,nunchuckStick.forward,Wii.GetAnalogStick(whichRemote).x*-30.0f);
					
					if(Wii.GetButton(whichRemote, "C"))
						buttonC.SetActive(true);
					else
						buttonC.SetActive(false);
					if(Wii.GetButton(whichRemote,"Z"))
						buttonZ.SetActive(true);
					else
						buttonZ.SetActive(false);
					
					inputDisplay = inputDisplay + "\nNUNCHUCK";
					inputDisplay = inputDisplay + "\nC       "+Wii.GetButton(whichRemote,"C").ToString();
					inputDisplay = inputDisplay + "\nZ       "+Wii.GetButton(whichRemote,"Z").ToString();
					inputDisplay = inputDisplay + "\nnunchuckStick N "+Wii.GetAnalogStick(whichRemote).ToString("#.0000");
					inputDisplay = inputDisplay + "\nAccel N "+Wii.GetNunchuckAcceleration(whichRemote).ToString("#.0000");
				}
				else if(Wii.GetExpType(whichRemote)==2)//classic controller is in
				{
					classic.gameObject.SetActive(true);
					Vector2 theStickLeft  = Wii.GetAnalogStick(whichRemote,"CLASSICLEFT");
					Vector2 theStickRight = Wii.GetAnalogStick(whichRemote,"CLASSICRIGHT");
					
					classicStickLeft.rotation = classic.rotation;
					classicStickLeft.RotateAround(classicStickLeft.position,transform.right,
					                              theStickLeft.y* 30.0f);
					classicStickLeft.RotateAround(classicStickLeft.position,transform.forward,
					                              theStickLeft.x*-30.0f); 
					
					classicStickRight.rotation = classic.rotation;
					classicStickRight.RotateAround(classicStickRight.position,transform.right,
					                               theStickRight.y* 30.0f);
					classicStickRight.RotateAround(classicStickRight.position,transform.forward,
					                               theStickRight.x*-30.0f); 
					
					buttonClassicL.transform.localScale = new Vector3(4.0f * Wii.GetAnalogButton(whichRemote, "CLASSICL"),buttonClassicL.transform.localScale.y,buttonClassicL.transform.localScale.z);
					buttonClassicR.transform.localScale = new Vector3(4.0f * Wii.GetAnalogButton(whichRemote, "CLASSICR"),buttonClassicL.transform.localScale.y,buttonClassicL.transform.localScale.z);
					
					buttonClassicA.SetActive     (Wii.GetButton(whichRemote,"CLASSICA"));
					buttonClassicB.SetActive     (Wii.GetButton(whichRemote,"CLASSICB"));
					buttonClassicMinus.SetActive (Wii.GetButton(whichRemote,"CLASSICMINUS"));
					buttonClassicPlus.SetActive  (Wii.GetButton(whichRemote,"CLASSICPLUS"));
					buttonClassicHome.SetActive  (Wii.GetButton(whichRemote,"CLASSICHOME"));
					buttonClassicX.SetActive     (Wii.GetButton(whichRemote,"CLASSICX"));
					buttonClassicY.SetActive     (Wii.GetButton(whichRemote,"CLASSICY"));
					buttonClassicUp.SetActive    (Wii.GetButton(whichRemote,"CLASSICUP"));
					buttonClassicDown.SetActive  (Wii.GetButton(whichRemote,"CLASSICDOWN"));
					buttonClassicLeft.SetActive  (Wii.GetButton(whichRemote,"CLASSICLEFT"));
					buttonClassicRight.SetActive (Wii.GetButton(whichRemote,"CLASSICRIGHT"));
					buttonClassicL.SetActive     (Wii.GetButton(whichRemote,"CLASSICL"));
					buttonClassicR.SetActive     (Wii.GetButton(whichRemote,"CLASSICR"));
					buttonClassicZL.SetActive    (Wii.GetButton(whichRemote,"CLASSICZL"));
					buttonClassicZR.SetActive    (Wii.GetButton(whichRemote,"CLASSICZR"));
					
					inputDisplay = inputDisplay + "\n CLASSIC";
					inputDisplay = inputDisplay + "\na       "+Wii.GetButton(whichRemote,"CLASSICA").ToString();    
					inputDisplay = inputDisplay + "\nb       "+Wii.GetButton(whichRemote,"CLASSICB").ToString();
					inputDisplay = inputDisplay + "\n-       "+Wii.GetButton(whichRemote,"CLASSICMINUS").ToString();
					inputDisplay = inputDisplay + "\n+       "+Wii.GetButton(whichRemote,"CLASSICPLUS").ToString();
					inputDisplay = inputDisplay + "\nhome    "+Wii.GetButton(whichRemote,"CLASSICHOME").ToString();
					inputDisplay = inputDisplay + "\nx       "+Wii.GetButton(whichRemote,"CLASSICX").ToString();
					inputDisplay = inputDisplay + "\ny       "+Wii.GetButton(whichRemote,"CLASSICY").ToString();
					inputDisplay = inputDisplay + "\nup      "+Wii.GetButton(whichRemote,"CLASSICUP").ToString();
					inputDisplay = inputDisplay + "\ndown    "+Wii.GetButton(whichRemote,"CLASSICDOWN").ToString();
					inputDisplay = inputDisplay + "\nleft    "+Wii.GetButton(whichRemote,"CLASSICLEFT").ToString();
					inputDisplay = inputDisplay + "\nright   "+Wii.GetButton(whichRemote,"CLASSICRIGHT").ToString();
					inputDisplay = inputDisplay + "\nL       "+Wii.GetButton(whichRemote,"CLASSICL").ToString();
					inputDisplay = inputDisplay + " "      +Wii.GetAnalogButton(whichRemote,"CLASSICL").ToString("#.000");
					inputDisplay = inputDisplay + "\nR       "+Wii.GetButton(whichRemote,"CLASSICR").ToString();
					inputDisplay = inputDisplay + " "      +Wii.GetAnalogButton(whichRemote,"CLASSICR").ToString("#.000");
					inputDisplay = inputDisplay + "\nZL      "+Wii.GetButton(whichRemote,"CLASSICZL").ToString();
					inputDisplay = inputDisplay + "\nZR      "+Wii.GetButton(whichRemote,"CLASSICZR").ToString();
					inputDisplay = inputDisplay + "\nStick L "+theStickLeft.ToString("#.0000");
					inputDisplay = inputDisplay + "\nStick R "+theStickRight.ToString("#.0000");	       
				}
				else if(Wii.GetExpType(whichRemote)==4)//guitar is in
				{
					guitarBody.gameObject.SetActive(true);
					var theStick  = Wii.GetAnalogStick(whichRemote,"GUITAR");
					var theWhammy = Wii.GetGuitarWhammy(whichRemote);
					var theStrum  = Wii.GetGuitarStrum(whichRemote);
					
					
					buttonGuitarGreen.SetActive(Wii.GetButton(whichRemote,"GUITARGREEN"));
					buttonGuitarRed.SetActive(Wii.GetButton(whichRemote,"GUITARRED"));
					buttonGuitarYellow.SetActive(Wii.GetButton(whichRemote,"GUITARYELLOW"));
					buttonGuitarBlue.SetActive(Wii.GetButton(whichRemote,"GUITARBLUE"));
					buttonGuitarOrange.SetActive(Wii.GetButton(whichRemote,"GUITARORANGE"));
					buttonGuitarPlus.SetActive(Wii.GetButton(whichRemote,"GUITARPLUS"));
					buttonGuitarMinus.SetActive(Wii.GetButton(whichRemote,"GUITARMINUS"));
					
					guitarBody.localRotation = Quaternion.Euler(0,0,0);
					guitarBody.RotateAround(guitarBody.position,guitarBody.forward,wiiAccel.x*90.0f);
					
					theStick = Wii.GetAnalogStick(whichRemote,"GUITAR");
					guitarStick.rotation = guitarBody.rotation;
					guitarStick.RotateAround(guitarStick.position, guitarStick.up,  theStick.y* 30.0f);
					guitarStick.RotateAround(guitarStick.position, guitarStick.right,theStick.x*-30.0f);
					
					
					guitarStrum.rotation = guitarBody.rotation;
					guitarStrum.RotateAround(guitarStrum.position,guitarStrum.up,20.0f*Wii.GetGuitarStrum(whichRemote));
					guitarWhammy.rotation = guitarBody.rotation;
					guitarWhammy.RotateAround(guitarWhammy.position,guitarWhammy.right,20.0f*Wii.GetGuitarWhammy(whichRemote));
					
					inputDisplay = inputDisplay + "\n GUITAR";
					inputDisplay = inputDisplay + "\nGreen  "+Wii.GetButton(whichRemote,"GUITARGREEN").ToString();
					inputDisplay = inputDisplay + "\nRed    "+Wii.GetButton(whichRemote,"GUITARRED").ToString();
					inputDisplay = inputDisplay + "\nYellow "+Wii.GetButton(whichRemote,"GUITARYELLOW").ToString();
					inputDisplay = inputDisplay + "\nBlue   "+Wii.GetButton(whichRemote,"GUITARBLUE").ToString();
					inputDisplay = inputDisplay + "\nOrange "+Wii.GetButton(whichRemote,"GUITARORANGE").ToString();
					inputDisplay = inputDisplay + "\nPlus   "+Wii.GetButton(whichRemote,"GUITARPLUS").ToString();
					inputDisplay = inputDisplay + "\nMinus  "+Wii.GetButton(whichRemote,"GUITARMINUS").ToString();
					inputDisplay = inputDisplay + "\nStick  "+theStick.ToString("#.0000");
					inputDisplay = inputDisplay + "\nWhammy "+theWhammy.ToString("#.0000");
					inputDisplay = inputDisplay + "\nStrum  "+theStrum.ToString();
					
				}
				else if(Wii.GetExpType(whichRemote)==5)//drums are in
				{
					drumSet.gameObject.SetActive(true);
					Vector2 theStick = Wii.GetAnalogStick(whichRemote,"DRUMS");
					drumStick.rotation = drumSet.rotation;
					drumStick.RotateAround(drumStick.position,  drumStick.right,
					                       theStick.y* 30.0f);
					drumStick.RotateAround(drumStick.position,drumStick.forward,
					                       theStick.x*-30.0f);
					
					drumPlus.enabled = Wii.GetButton(whichRemote,"DRUMPLUS");
					drumMinus.enabled = Wii.GetButton(whichRemote,"DRUMMINUS");
					
					if(Wii.GetButton(whichRemote,"DRUMGREEN"))
					{
						float boom = 1.5f/Wii.GetDrumVelocity(whichRemote,"GREEN");
						drumGreen.localScale = new Vector3(boom,drumGreen.localScale.y,boom);
					}
					else
					{
						float boom = Mathf.Lerp(drumGreen.localScale.x,1.5f,.1f);
						drumGreen.localScale = new Vector3(boom,drumGreen.localScale.y,boom);
					}
					
					if(Wii.GetButton(whichRemote,"DRUMBLUE"))
					{
						float boom = 1.5f/Wii.GetDrumVelocity(whichRemote,"BLUE");
						drumBlue.localScale = new Vector3(boom,drumBlue.localScale.y,boom) ;
					}
					else
					{
						float boom = Mathf.Lerp(drumBlue.localScale.x,1.5f,.1f);
						drumBlue.localScale = new Vector3(boom,drumBlue.localScale.y,boom);
					}
					
					if(Wii.GetButton(whichRemote,"DRUMRED"))
					{
						float boom = 1.5f/Wii.GetDrumVelocity(whichRemote,"RED");
						drumRed.localScale = new Vector3(boom,drumRed.localScale.y,boom);
					}
					else
					{
						float boom = Mathf.Lerp(drumRed.localScale.x,1.5f,.1f);
						drumRed.localScale = new Vector3(boom,drumRed.localScale.y,boom);
					}
					
					if(Wii.GetButton(whichRemote,"DRUMORANGE"))
					{
						float boom = 1.5f/Wii.GetDrumVelocity(whichRemote,"ORANGE");
						drumOrange.localScale = new Vector3(boom,drumOrange.localScale.y,boom);
					}
					else
					{
						float boom = Mathf.Lerp(drumOrange.localScale.x,1.5f,.1f);
						drumOrange.localScale = new Vector3(boom,drumOrange.localScale.y,boom);
					}
					
					if(Wii.GetButton(whichRemote,"DRUMYELLOW"))
					{
						float boom = 1.5f/Wii.GetDrumVelocity(whichRemote,"YELLOW");
						drumYellow.localScale = new Vector3(boom,drumYellow.localScale.y,boom);
					}
					else
					{
						float boom = Mathf.Lerp(drumYellow.localScale.x,1.5f,.1f);
						drumYellow.localScale = new Vector3(boom,drumYellow.localScale.y,boom);
					}
					
					inputDisplay = inputDisplay + "\n  DRUMS";
					inputDisplay = inputDisplay + "\nGreen  "+Wii.GetButton(whichRemote,"DRUMGREEN").ToString();
					inputDisplay = inputDisplay + "\nRed    "+Wii.GetButton(whichRemote,"DRUMRED").ToString();
					inputDisplay = inputDisplay + "\nYellow "+Wii.GetButton(whichRemote,"DRUMYELLOW").ToString();
					inputDisplay = inputDisplay + "\nBlue   "+Wii.GetButton(whichRemote,"DRUMBLUE").ToString();
					inputDisplay = inputDisplay + "\nOrange "+Wii.GetButton(whichRemote,"DRUMORANGE").ToString();
					inputDisplay = inputDisplay + "\nPlus   "+Wii.GetButton(whichRemote,"DRUMPLUS").ToString();
					inputDisplay = inputDisplay + "\nMinus  "+Wii.GetButton(whichRemote,"DRUMMINUS").ToString();
					inputDisplay = inputDisplay + "\nPedal  "+Wii.GetButton(whichRemote,"DRUMPEDAL").ToString();
					inputDisplay = inputDisplay + "\nStick  "+theStick.ToString("#.0000");
					
				}
				else if(Wii.GetExpType(whichRemote)==6)//turntable is in
				{
					Vector2 theStick = Wii.GetAnalogStick(whichRemote,"TURNTABLE");
					hub.gameObject.SetActive(true);
					
					dial.rotation = hub.rotation;
					dial.RotateAround(dial.position,dial.up, 360.0f * Wii.GetTurntableDial(whichRemote));
					
					slider.localPosition = new Vector3(.3f * Wii.GetTurntableSlider(whichRemote),slider.localPosition.y,slider.localPosition.z);
					
					
					tableStick.rotation = hub.rotation;
					tableStick.RotateAround(tableStick.position,  tableStick.right,
					                        theStick.y* 30.0f);
					tableStick.RotateAround(tableStick.position,tableStick.forward,
					                        theStick.x*-30.0f);
					
					buttonTablePlus.SetActive(Wii.GetButton(whichRemote,"TURNTABLEPLUS"));
					buttonTableMinus.SetActive(Wii.GetButton(whichRemote,"TURNTABLEMINUS"));
					butttonEuphoria.SetActive(Wii.GetButton(whichRemote,"TURNTABLEEUPHORIA"));
					
					platterLeft.RotateAround( platterLeft.position, platterLeft.up, Wii.GetTurntableSpin(whichRemote,"LEFT") );
					platterRight.RotateAround(platterRight.position,platterRight.up,Wii.GetTurntableSpin(whichRemote,"RIGHT"));
					
					tableLeftGreen.SetActive(Wii.GetButton(whichRemote,"TURNTABLEGREENLEFT"));
					tableLeftRed.SetActive(Wii.GetButton(whichRemote,"TURNTABLEREDLEFT"));
					tableLeftBlue.SetActive(Wii.GetButton(whichRemote,"TURNTABLEBLUELEFT"));
					tableRightGreen.SetActive(Wii.GetButton(whichRemote,"TURNTABLEGREENRIGHT"));
					tableRightRed.SetActive(Wii.GetButton(whichRemote,"TURNTABLEREDRIGHT"));
					tableRightBlue.SetActive(Wii.GetButton(whichRemote,"TURNTABLEBLUERIGHT"));
					
					inputDisplay = inputDisplay + "\nTURN TABLE";
					inputDisplay = inputDisplay + "\nLeft Green  "+Wii.GetButton(whichRemote,"TURNTABLEGREENLEFT").ToString();
					inputDisplay = inputDisplay + "\nLeft Red    "+Wii.GetButton(whichRemote,"TURNTABLEREDLEFT").ToString();
					inputDisplay = inputDisplay + "\nLeft Blue   "+Wii.GetButton(whichRemote,"TURNTABLEBLUELEFT").ToString();
					inputDisplay = inputDisplay + "\nRight Green   "+Wii.GetButton(whichRemote,"TURNTABLEGREENRIGHT").ToString();
					inputDisplay = inputDisplay + "\nRight Red     "+Wii.GetButton(whichRemote,"TURNTABLEREDRIGHT").ToString();
					inputDisplay = inputDisplay + "\nRight Blue    "+Wii.GetButton(whichRemote,"TURNTABLEBLUERIGHT").ToString();
					inputDisplay = inputDisplay + "\nEuphoria    "+Wii.GetButton(whichRemote,"TURNTABLEEUPHORIA").ToString();																inputDisplay = inputDisplay + "\nPlus        "+Wii.GetButton(whichRemote,"TURNTABLEPLUS").ToString();
					inputDisplay = inputDisplay + "\nMinus       "+Wii.GetButton(whichRemote,"TURNTABLEMINUS").ToString();
					inputDisplay = inputDisplay + "\nStick       "+theStick.ToString("#.0000");
					inputDisplay = inputDisplay + "\nSlider      "+Wii.GetTurntableSlider(whichRemote);
					inputDisplay = inputDisplay + "\nDial        "+Wii.GetTurntableDial(whichRemote);
					inputDisplay = inputDisplay + "\nLeft Table  "+Wii.GetTurntableSpin(whichRemote,"LEFT");
					inputDisplay = inputDisplay + "\nRight Table "+Wii.GetTurntableSpin(whichRemote,"RIGHT");;
				}
				else if(Wii.GetExpType(whichRemote)==7)//WiiUPro is in
				{
					proU.gameObject.SetActive(true);
					wiimote.gameObject.SetActive(false);

					detectMotionButton.SetActive(false);
					calibrateWiiUProButton.SetActive(true);
					Vector2 theStickLeft  = Wii.GetAnalogStick(whichRemote,"WIIUPROLEFT");
					Vector2 theStickRight = Wii.GetAnalogStick(whichRemote,"WIIUPRORIGHT");

					proU.gameObject.SetActive(true);
					
					proUStickLeft.rotation = proU.rotation;
					proUStickLeft.RotateAround(proUStickLeft.position,transform.right,
					                              theStickLeft.y* 30.0f);
					proUStickLeft.RotateAround(proUStickLeft.position,transform.forward,
					                              theStickLeft.x*-30.0f); 
					
					proUStickRight.rotation = proU.rotation;
					proUStickRight.RotateAround(proUStickRight.position,transform.right,
					                               theStickRight.y* 30.0f);
					proUStickRight.RotateAround(proUStickRight.position,transform.forward,
					                               theStickRight.x*-30.0f); 

					if(Wii.GetButton(whichRemote,"WIIUPROL3"))
					{
						proUStickLeft.localScale = new Vector3 (proUStickLeft.localScale.x,1.5f,proUStickLeft.localScale.z);
					}
					else
					{
						proUStickLeft.localScale = new Vector3 (proUStickLeft.localScale.x,2.3f,proUStickLeft.localScale.z);
					}
					
					if(Wii.GetButton(whichRemote,"WIIUPROR3"))
					{
						proUStickRight.localScale = new Vector3 (proUStickRight.localScale.x,1.5f,proUStickRight.localScale.z);
					}
					else
					{
						proUStickRight.localScale = new Vector3 (proUStickRight.localScale.x,2.3f,proUStickRight.localScale.z);
					}

					buttonProUA.SetActive     (Wii.GetButton(whichRemote,"WIIUPROA"));
					buttonProUB.SetActive     (Wii.GetButton(whichRemote,"WIIUPROB"));
					buttonProUMinus.SetActive (Wii.GetButton(whichRemote,"WIIUPROMINUS"));
					buttonProUPlus.SetActive  (Wii.GetButton(whichRemote,"WIIUPROPLUS"));
					buttonProUHome.SetActive  (Wii.GetButton(whichRemote,"WIIUPROHOME"));
					buttonProUX.SetActive     (Wii.GetButton(whichRemote,"WIIUPROX"));
					buttonProUY.SetActive     (Wii.GetButton(whichRemote,"WIIUPROY"));
					buttonProUUp.SetActive    (Wii.GetButton(whichRemote,"WIIUPROUP"));
					buttonProUDown.SetActive  (Wii.GetButton(whichRemote,"WIIUPRODOWN"));
					buttonProULeft.SetActive  (Wii.GetButton(whichRemote,"WIIUPROLEFT"));
					buttonProURight.SetActive (Wii.GetButton(whichRemote,"WIIUPRORIGHT"));
					buttonProUL.SetActive     (Wii.GetButton(whichRemote,"WIIUPROL"));
					buttonProUR.SetActive     (Wii.GetButton(whichRemote,"WIIUPROR"));
					buttonProUZL.SetActive    (Wii.GetButton(whichRemote,"WIIUPROZL"));
					buttonProUZR.SetActive    (Wii.GetButton(whichRemote,"WIIUPROZR"));


					inputDisplay = " WII U PRO ";
					inputDisplay = inputDisplay + "\na       "+Wii.GetButton(whichRemote,"WIIUPROA").ToString();    
					inputDisplay = inputDisplay + "\nb       "+Wii.GetButton(whichRemote,"WIIUPROB").ToString();
					inputDisplay = inputDisplay + "\n-       "+Wii.GetButton(whichRemote,"WIIUPROMINUS").ToString();
					inputDisplay = inputDisplay + "\n+       "+Wii.GetButton(whichRemote,"WIIUPROPLUS").ToString();
					inputDisplay = inputDisplay + "\nhome    "+Wii.GetButton(whichRemote,"WIIUPROHOME").ToString();
					inputDisplay = inputDisplay + "\nx       "+Wii.GetButton(whichRemote,"WIIUPROX").ToString();
					inputDisplay = inputDisplay + "\ny       "+Wii.GetButton(whichRemote,"WIIUPROY").ToString();
					inputDisplay = inputDisplay + "\nup      "+Wii.GetButton(whichRemote,"WIIUPROUP").ToString();
					inputDisplay = inputDisplay + "\ndown    "+Wii.GetButton(whichRemote,"WIIUPRODOWN").ToString();
					inputDisplay = inputDisplay + "\nleft    "+Wii.GetButton(whichRemote,"WIIUPROLEFT").ToString();
					inputDisplay = inputDisplay + "\nright   "+Wii.GetButton(whichRemote,"WIIUPRORIGHT").ToString();
					inputDisplay = inputDisplay + "\nL       "+Wii.GetButton(whichRemote,"WIIUPROL").ToString();
					inputDisplay = inputDisplay + "\nR       "+Wii.GetButton(whichRemote,"WIIUPROR").ToString();
					inputDisplay = inputDisplay + "\nZL      "+Wii.GetButton(whichRemote,"WIIUPROZL").ToString();
					inputDisplay = inputDisplay + "\nZR      "+Wii.GetButton(whichRemote,"WIIUPROZR").ToString();
					inputDisplay = inputDisplay + "\nL3      "+Wii.GetButton(whichRemote,"WIIUPROL3").ToString();
					inputDisplay = inputDisplay + "\nR3      "+Wii.GetButton(whichRemote,"WIIUPROR3").ToString();
					inputDisplay = inputDisplay + "\nStick L "+theStickLeft.ToString("#.0000");
					inputDisplay = inputDisplay + "\nStick R "+theStickRight.ToString("#.0000");
				}
				else if(Wii.GetExpType(whichRemote)==0)
				{
					nunchuk.gameObject.SetActive(false);
					balanceBoard.gameObject.SetActive(false);
					classic.gameObject.SetActive(false);
					guitarBody.gameObject.SetActive(false);
					hub.gameObject.SetActive(false);
					drumSet.gameObject.SetActive(false);
					proU.gameObject.SetActive(false);

				}
			}
			theText.text=inputDisplay;	
		}
		else
		{
			dropButton.SetActive(false);

			nunchuk.gameObject.SetActive(false);
			balanceBoard.gameObject.SetActive(false);
			classic.gameObject.SetActive(false);
			guitarBody.gameObject.SetActive(false);
			hub.gameObject.SetActive(false);
			drumSet.gameObject.SetActive(false);
			wiimote.gameObject.SetActive(false);
			motionPlus.gameObject.SetActive(false);
			proU.gameObject.SetActive(false);

			calibrateWiiUProButton.SetActive(false);
			detectMotionButton.SetActive(false);
			calibrateMotionButton.SetActive(false);
			uncalibrateMotionButton.SetActive(false);
			deactivateMotionButton.SetActive(false);
				

			theRemoteNumber.enabled=false;
			theText.text = "Ready.";
		}
		
		if(Wii.IsSearching())
		{
			//cancel search	
			cancelButton.SetActive(true);
		}
		else
		{
			cancelButton.SetActive(false);
			if (!Wii.IsSearching() && (totalRemotes<16)) {
				//Find Button
				searchButton.SetActive(true);
			}
			else
			{
				searchButton.SetActive(false);
			}
		}

	}

	public void ChooseRemote(Button remotebutton)
	{
		whichRemote = int.Parse (remotebutton.gameObject.name);

		if(!Wii.IsActive(whichRemote))
		{
			if(Wii.IsSearching())
				theText.text = ("I'm looking.");
			else
				theText.text = ("remote "+whichRemote+" is not active.");
		}
	}

	public void DropRemote()
	{
		Wii.DropWiiRemote(whichRemote);   
		theText.text = ("Dropped "+whichRemote);
	}

	public void DeactivateMotionPlus()
	{
		Wii.DeactivateMotionPlus(whichRemote);
	}

	public void CalibrateMotionPlus()
	{
		Wii.CalibrateMotionPlus(whichRemote);
	}

	public void UncalibrateMotionPlus()
	{
		Wii.UncalibrateMotionPlus(whichRemote);
	}

	public void CheckForMotionPlus()
	{
		Wii.CheckForMotionPlus(whichRemote);
	}

	public void CalibrateWiiUPro()
	{
		Wii.CalibrateWiiUPro(whichRemote);
	}

	public void CancelSearch()
	{
		//searching = false;
		Wii.StopSearch();   
		theText.text = "Cancelled.";
	}

	public void BeginSearch()
	{
		//searching = true;
		Wii.StartSearch();   
		theText.text = "I'm looking.";
		Time.timeScale = 1.0f;
	}

	public void OnDiscoveryFailed(int i) {
		theText.text = "Error:"+i+". Try Again.";
		//searching = false;
	}
	
	public void OnWiimoteDiscovered (int thisRemote) {
		Debug.Log("found this one: "+thisRemote);
		if(!Wii.IsActive(whichRemote))
			whichRemote = thisRemote;
	}
	
	public void OnWiimoteDisconnected (int whichRemote) {
		Debug.Log("lost this one: "+ whichRemote);	
	}
}