using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using PrattiToolkit;

public class TCPCom : MonoBehaviour
{
    #region editor
    [Space]
	[Header("TCP Communication")]
	public string IP_Seat = "192.168.50.30";
	public int port = 34197;

	[Space]
	//[Header(" Valori in ingresso al sistema")]
	[HideInInspector] public float dX = 0.0f;
	[HideInInspector] public float dY = 0.0f;

	[HideInInspector] public float x = 0.0f;
	[HideInInspector] public float y = 0.0f;

	[Header(" Valori in uscita al sistema")]
	public float x_out = 0.0f;
	public float y_out = 0.0f;
	public Vector2 movement = Vector2.zero;
	public float angleRotation = 0.0f;


	[Space]
	[Header("Sensibilità cambio direzione VA")]
	[Tooltip("_TH controllano la sensibilit� ai movimenti lungo l'asse indicato")]
	//public float dxPrediction_TH = 0.01f; // setta la sensibilit� della seduta ai movimenti del corpo
	public float dyPrediction_TH = 0.01f;  // setta la sensibilit� della seduta ai movimenti del corpo
	[Space]
	[Header("Sensibilità passaggio camminata cors %")]
	public float slopeWalkRun = 0.85f;

	[Space]
	[Header("Posizione baricentro per stasi (%)")]
	[Tooltip("_TH controllano la distanza del baricentro dal centro per distinguere stasi e movimento (0,1) ")]
	public float stasiFR_TH = .20f;
	public float stasiLR_TH = .25f;  // setta la sensibilit� della seduta per distinguere stasi e movimento

	[Space]
	//[Header("Denoiser")]
	//[Tooltip("consigliato: win_DNS = #pack-30+1")]
	private int win_DNS = 7;
	public AnimationCurve curveFR, curveLR;
	//public float[] valuesFR = new float[] {0, 1/3f, 1/3f, 2/3f, 1 };
	public float[] valuesFR = new float[] { 0, 1 / 3f, 1 / 3f, 2/3f };
	[Tooltip("higher number = flatter curve on the left ")]
	public float curveWeightB = 0.66f;
	[Tooltip("higher number = flatter curve on the right")]
	public float curveWeightF = 0.33f;

	[Space]
	[Header("Debug")]
	public bool showX = true;
	public bool showY = true;
	public bool debug = false;
	public bool freeXmovement = true;
    #endregion

    #region private members 	
    private TcpClient socketConnection;
	private Thread clientReceiveThread;


	private float a, b, c, d = 0.0f;
	[HideInInspector] public string serverMessage;
	private int timeWin = 28;
	private float vel = 0.0f;

	private List<string> dataIO = new List<string>();
	private string _seatDataFile;

	private Queue<float> dx_Q = new Queue<float>();
	private Queue<float> dy_Q = new Queue<float>();

	[HideInInspector] public float totMass = 0;
	private float x_tare, y_tare, dY_old = 0.0f;
	private SimplePID simplePID;
    private PIDPars _pidPars;
	private float setpoint = 0;
    #endregion

    #region eventFunc
    void Awake()
	{
		/*
#if (!UNITY_EDITOR)
		IP_Seat = ConfigurationLookUp.Instance.GetString("WalkingSeatIP", "0.0.0.0");
#endif
		*/

		_pidPars = new PIDPars();
			
		_pidPars.Kp = 1f;
		_pidPars.Ki = 0f;
		_pidPars.Kd = 1.5f;
			
		simplePID = new SimplePID(_pidPars);

		IP_Seat = "192.168.50.30"; //  Solo se connesso a XRNetwork
		ConnectToTcpServer();


		Calibrate();
		CreateCurveFRAsym(0);
		CreateCurveLR(stasiLR_TH);
	

		if (debug)
		{
			string _logsDirectory = Path.Combine(Application.dataPath, "SessionLogs");
			if (!Directory.Exists(_logsDirectory))
				Directory.CreateDirectory(_logsDirectory);
			_seatDataFile = Path.Combine(_logsDirectory, "dataSeatIO.txt");

		}
		//start = true;
		//startEditor = true;
	}

	private void OnDisable()
	{
		if (debug)
		{
			try
			{
			/*	File.AppendAllText(_seatDataFile,
					"TimeStamp a b c d " +
					"x y dx dy " +
					"xout yout " +
					"angRot" +
					" " + dyPrediction_TH +
					" " + stasiLR_TH +
					" " + stasiFR_TH + "\n");*/
				File.AppendAllText(_seatDataFile, string.Join("", dataIO.ToArray()));
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}


		if (socketConnection != null)
		{
			if (socketConnection.Connected)
				socketConnection.Close();

			socketConnection.Dispose();
		}

		if (clientReceiveThread != null)
			clientReceiveThread.Abort();
	}
    #endregion

    #region Connection
    private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
		}
	}
	private void ListenForData()
	{
		try
		{
			socketConnection = new TcpClient(IP_Seat, port);
			Byte[] bytes = new Byte[36];
			while (socketConnection.Connected)
			{
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length = 0;
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						if (length == 36)
						{
							var incommingData = new byte[length];

							var X = new Byte[4];
							var Y = new Byte[4];
							var DX = new Byte[4];
							var DY = new Byte[4];

							var A = new Byte[4];
							var B = new Byte[4];
							var C = new Byte[4];
							var D = new Byte[4];

							var VEL = new Byte[4];

							Array.Copy(bytes, 0, incommingData, 0, length);
							serverMessage = Encoding.ASCII.GetString(incommingData);

							Array.Copy(bytes, 0, X, 0, 4);
							Array.Copy(bytes, 4, Y, 0, 4);
							Array.Copy(bytes, 8, DX, 0, 4);
							Array.Copy(bytes, 12, DY, 0, 4);

							Array.Copy(bytes, 16, A, 0, 4);
							Array.Copy(bytes, 20, B, 0, 4);
							Array.Copy(bytes, 24, C, 0, 4);
							Array.Copy(bytes, 28, D, 0, 4);

							Array.Copy(bytes, 32, VEL, 0, 4);

							x = (float)BitConverter.ToSingle(X, 0);
							y = (float)BitConverter.ToSingle(Y, 0);
							dX = (float)BitConverter.ToSingle(DX, 0);
							dY = (float)BitConverter.ToSingle(DY, 0);

							a = BitConverter.ToSingle(A, 0);
							b = BitConverter.ToSingle(B, 0);
							c = BitConverter.ToSingle(C, 0);
							d = BitConverter.ToSingle(D, 0);

							vel = BitConverter.ToSingle(VEL, 0);

							//TARA

							totMass = Mathf.Abs(a + b + c + d) > totMass ? Mathf.Abs(a + b + c + d) : totMass;

#if !UNITY_EDITOR
							y = y - LocomotionManager.Instance.CalibrationData.WSYCenter; //usare ytqare da locomotioonmanager
#endif
							//DENOISING
							dY_old=dY;
							dX = Denoise_AVERAGE(dx_Q, dX, win_DNS);
							dY = Denoise_AVERAGE(dy_Q, dY, win_DNS);

							
							MovementCompPID();
							//DEBUG
							DebugData();

						}

					}

				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}
	private float Denoise_AVERAGE(Queue<float> queue, float value, int windowDenoise)
	{
		float sum = 0;
		float[] sumArr = new float[windowDenoise];

		queue.Enqueue(value);

		if (queue.Count > windowDenoise) queue.Dequeue();
		queue.CopyTo(sumArr, 0);

		for (int i = 0; i < windowDenoise; i++)
		{
			sum += sumArr[i];
		}
		sum = Mathf.Abs(sum / windowDenoise) > 0.001f ? (sum / windowDenoise) : 0;
		return sum;
	}
	#endregion

	#region curve manager
	/*private void CreateCurveFR(AnimationCurve curve, float _TH)
	{
		curve.AddKey(_TH, 0);
		curve.AddKey(_TH + (1 - _TH + _TH * 0.1f) / 2, .5f);
		curve.AddKey(.9f, 1);

		Keyframe newkey = new Keyframe();
		newkey.time = curve.keys[0].time;
		newkey.value = curve.keys[0].value;
		newkey.inTangent = 0f;
		newkey.outTangent = 0f;
		newkey.weightedMode = WeightedMode.None;
		newkey.inWeight = 0f;
		newkey.outWeight = 0f;
		curve.MoveKey(0, newkey);

		newkey = new Keyframe();
		newkey.time = curve.keys[1].time;
		newkey.value = curve.keys[1].value;
		newkey.inTangent = 0f;
		newkey.outTangent = 0f;
		newkey.weightedMode = WeightedMode.None;
		newkey.inWeight = 0f;
		newkey.outWeight = 0f;
		curve.MoveKey(1, newkey);

		newkey = new Keyframe();
		newkey.time = curve.keys[2].time;
		newkey.value = curve.keys[2].value;
		newkey.inTangent = 0f;
		newkey.outTangent = 0f;
		newkey.weightedMode = WeightedMode.None;
		newkey.inWeight = 0f;
		newkey.outWeight = 0f;
		curve.MoveKey(2, newkey);
	} // deprecated
	*/
	public void CreateCurveFRAsym(float slopeControl)
	{
		AnimationCurve curve = curveFR;
		float _TH = stasiFR_TH;

		curve.AddKey(-1, valuesFR[0] );
		curve.AddKey(-_TH, valuesFR[1]);
		curve.AddKey(_TH, valuesFR[2]);
		curve.AddKey(1, valuesFR[3]);
		//curve.AddKey(_TH + (1 - _TH) * slopeControl, valuesFR[3]);
		//curve.AddKey(2f, valuesFR[4]);

		for (int key = 0; key < curve.keys.Length; key++)
		{
			Keyframe newkey = new Keyframe();
			newkey.time = curve.keys[key].time;
			newkey.value = curve.keys[key].value;
			newkey.inTangent = 0f;
			newkey.outTangent = 0f;
			newkey.weightedMode = WeightedMode.Both;
			newkey.inWeight =  (key==1)? curveWeightB :  .33f;
			newkey.outWeight = (key == 2) ? curveWeightF : .33f; 
			curve.MoveKey(key, newkey);
		}
	}
	public void resetCurveFR()
	{
		AnimationCurve curve = curveFR;

		for (int key = 0; key < curve.keys.Length; key++)
		{
			Keyframe newkey = new Keyframe();
			newkey.time = curve.keys[key].time;
			newkey.value = valuesFR[key];
			newkey.inTangent = 0f;
			newkey.outTangent = 0f;
			newkey.weightedMode = WeightedMode.Both;
			newkey.inWeight = (key == 1) ? curveWeightB : .33f;
			newkey.outWeight = (key == 2) ? curveWeightF : .33f;
			curve.MoveKey(key, newkey);
		}
	}
	public void ModCurveFRAsym(float speedControl)
    {
		AnimationCurve curve = curveFR;
		float _TH = stasiFR_TH;

		Keyframe modkey = new Keyframe();
		//modkey.time = _TH + Mathf.Clamp((1 - _TH) * slopeControl,0.1f ,1-_TH-0.1f );
		//modkey.value = curve.keys[curve.keys.Length-2].value;
		modkey.time = curve.keys[curve.keys.Length - 1].time;
		modkey.value = curve.keys[curve.keys.Length - 1].value * (1+speedControl*1/2f);
		modkey.inTangent = 0f;
		modkey.outTangent = 0f;
		modkey.weightedMode = WeightedMode.Both;
		modkey.inWeight = 0f;
		modkey.outWeight = 0f;
		curve.MoveKey(curve.keys.Length - 1, modkey);

		for (int key = 0; key < curve.keys.Length; key++)
		{

			Keyframe newkey = new Keyframe();
			newkey.time = curve.keys[key].time;
			newkey.value = curve.keys[key].value;
			newkey.inTangent = 0f;
			newkey.outTangent = 0f;
			newkey.weightedMode = WeightedMode.Both;
			newkey.inWeight = (key == 1) ? curveWeightB : .33f;
			newkey.outWeight = (key == 2) ? curveWeightF : .33f;
			curve.MoveKey(key, newkey);
		}

	}
	public void ModCurveFRDeadZone(float deadZoneLimit)
	{

		AnimationCurve curve = curveFR;
		float _TH = stasiFR_TH;


		Keyframe modkey = new Keyframe();
		//modkey.time = _TH + Mathf.Clamp((1 - _TH) * slopeControl,0.1f ,1-_TH-0.1f );
		//modkey.value = curve.keys[curve.keys.Length-2].value;
		modkey.time = curve.keys[curve.keys.Length - 1].time*deadZoneLimit;
		modkey.value = curve.keys[curve.keys.Length - 1].value;
		modkey.inTangent = 0f;
		modkey.outTangent = 0f;
		modkey.weightedMode = WeightedMode.Both;
		modkey.inWeight = 0f;
		modkey.outWeight = 0f;
		curve.MoveKey(curve.keys.Length - 1, modkey);

		for (int key = 0; key < curve.keys.Length; key++)
		{

			Keyframe newkey = new Keyframe();
			newkey.time = curve.keys[key].time;
			newkey.value = curve.keys[key].value;
			newkey.inTangent = 0f;
			newkey.outTangent = 0f;
			newkey.weightedMode = WeightedMode.Both;
			newkey.inWeight = (key == 1) ? curveWeightB : .33f;
			newkey.outWeight = (key == 2) ? curveWeightF : .33f;
			curve.MoveKey(key, newkey);
		}

	}
	public void FlattenFrontAsym(float slopeControl)
	{
		resetCurveFR();
		AnimationCurve curve = curveFR;

		for (int key = 0; key < curve.keys.Length; key++)
		{
			if ( key > 0)
            {
				Keyframe newkey = new Keyframe();
				newkey.time = curve.keys[key].time;
				newkey.value = valuesFR[1];
				newkey.inTangent = 0f;
				newkey.outTangent = 0f;
				newkey.weightedMode = WeightedMode.None;
				newkey.inWeight = 0f;
				newkey.outWeight = 0f;
				curve.MoveKey(key, newkey);
			}
		}
	}
	public void FlattenRearAsym(float slopeControl)
	{
		resetCurveFR();
		AnimationCurve curve = curveFR;
		for (int key = 0; key < curve.keys.Length; key++)
		{
			if (key < 1)
			{
				Keyframe newkey = new Keyframe();
				newkey.time = curve.keys[key].time;
				newkey.value = valuesFR[1];
				newkey.inTangent = 0f;
				newkey.outTangent = 0f;
				newkey.weightedMode = WeightedMode.None;
				newkey.inWeight = 0f;
				newkey.outWeight = 0f;
				curve.MoveKey(key, newkey);
			}
		}
	}
	private void CreateCurveLR( float _TH)
	{
		AnimationCurve curve = curveLR;
		curve.AddKey(_TH, 0);
		curve.AddKey(1, 1);

		for (int key = 0; key < curve.keys.Length; key++)
		{

			Keyframe newkey = new Keyframe();
			newkey.time = curve.keys[key].time;
			newkey.value = curve.keys[key].value;
			newkey.inTangent = 0f;
			newkey.outTangent = 0f;
			newkey.weightedMode = WeightedMode.Both;
			newkey.inWeight = .33f;
			newkey.outWeight = .33f;
			curve.MoveKey(key, newkey);
		}

	}
	private void DeadZoneController()
    {

    }
    #endregion

    #region movement 
    public float Calibrate()
	{
		y_tare = y;
		return y_tare;
	}
	private void DebugData()
	{
		if (debug)
		{
			dataIO.Add(DateTime.Now.Millisecond + ";" +
				   Convert.ToString(x) + ";" +
				   Convert.ToString(y) + ";" +
				   Convert.ToString(dX*10) + ";" +
				   Convert.ToString(dY*10) + ";" +
				   Convert.ToString(x_out) + ";" +
				   Convert.ToString(y_out) + ";" +
				   Convert.ToString(movement.x*100) + ";" +
				   Convert.ToString(movement.y*100) + ";" +
				   Convert.ToString(angleRotation) + "\n");
		}
	}

	private void MovementCompPID()
	{

		x_out = x;// simplePID.UpdatePars(setpoint, x, timeWin);
		y_out = - simplePID.UpdatePars(setpoint, y, timeWin);

		if (Mathf.Abs(y) > Mathf.Abs(x))
		{
			movement = new Vector2(Mathf.Sign(x_out) * curveLR.Evaluate(Mathf.Abs(x_out) / (totMass)),	curveFR.Evaluate((y_out > 0 ? y_out * 2f : y_out) / (totMass))); 
		}
		else
		{
			movement = new Vector2(0, valuesFR[1]);
		}

		angleRotation = -vel;
	}
	private void MovementComp()
	{
        if (showX)
		{
			if (freeXmovement || Mathf.Abs(dX) > 0)
			{
				if (x * dX < 0)
				{
					if (Mathf.Abs(dX) > dyPrediction_TH)  // todo, controllare sensibilità ed eventuaklmenten aggiungere controllo su x,y
					{
						x_out = dX * totMass / 10;
					}
					else
					{
						x_out = x;
					}
				}
				else if (x * dX >= 0)
				{
					x_out = Mathf.Sign(x) * Mathf.Max(Mathf.Abs(dX * totMass / 10), Mathf.Abs(x));
				}

			}

		}else        {
			x_out = 0f;	
        }

		if ( showY)
		{
			if (y * dY <= 0)
			{
				if (Mathf.Abs(dY) > dyPrediction_TH || y == 0)
				{
					y_out = dY * totMass / 10;  
				}
				else
				{
					y_out = y;
				}
			}
			else if (y * dY > 0)
			{
				y_out = Mathf.Sign(y) * Mathf.Max(Mathf.Abs(dY * totMass / 10), Mathf.Abs(y));

			}

		}


		if (Mathf.Abs(y) > Mathf.Abs(x))
		{
			movement = new Vector2(Mathf.Sign(x_out) * curveLR.Evaluate(Mathf.Abs(x_out) / (totMass)),
								    curveFR.Evaluate((y_out > 0? y_out*2f : y_out) / (totMass)));  //todo_lv: verificare distirbuzione fvalori x_out, y_out 
		}
		else
		{
			movement = new Vector2(0, valuesFR[1]);
		}

		angleRotation = -vel;
	}
    #endregion
}