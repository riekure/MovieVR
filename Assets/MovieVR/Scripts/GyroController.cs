using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GyroController : MonoBehaviour
{
    // キーボード操作用
#if UNITY_EDITOR || UNITY_STANDALONE
    private Vector3 rotate;
#endif

    // 視点リセット用修正角度変数
    public Vector3 fixrot;
    // 視点リセット用現在角度変数
    public Vector3 nowrot;
    // 視点リセット用オフセット変数
    public Vector3 delrot;

    public VideoPlayer videoPlayer;
    // true:再生/false:一時停止
    public bool playModeFlag = false;

    public GameObject exitButtonOBJ;
    public GameObject playButtonOBJ;
    public GameObject resetButtonOBJ;
    public GameObject modeButtonOBJ;

    private Button playButton;
    public Sprite play_image;
    public Sprite pause_image;

    public float visibleTimeCount = 2.0f;
    public float timeCount = 0.0f;

    // VR操作モード:true/タッチ操作モード:false
    public bool modeFlag;
    public GameObject targetObject;

    // カメラの最小角度
    public float minCameraAngleX = 340.0f;
    // カメラの最大角度
    public float maxCameraAngleX = 20.0f;
    // スワイプで回転するときのスピード
    public float swipeTurnSpeed = 10.0f;

    // 基準となるタップの座標
    private Vector3 baseMousePos;
    // 基準となるカメラの座標
    private Vector3 baseCameraPos;
    // マウスが押下されているかフラグ
    private bool isMouseDown = false;

    private Button modeButton;
    public Sprite grayModeButton_image;
    public Sprite whiteModeButton_image;

    // Start is called before the first frame update
    void Start()
    {
        // 動作確認用ログ
        Debug.Log("started");


        fixrot = new Vector3(0, 0, 0);
        nowrot = new Vector3(0, 0, 0);
        delrot = new Vector3(0, 0, 0);

        playButton = playButtonOBJ.GetComponent<Button>();
        modeButton = modeButtonOBJ.GetComponent<Button>();
        timeCount = visibleTimeCount;
        SetPlayMode(playModeFlag);
        modeFlag = true;

#if UNITY_EDITOR || UNITY_STANDALONE
        rotate = transform.eulerAngles;
        // 動作環境の判別用ログ
        Debug.Log("non-smartphone");
#else
        Input.gyro.enabled = true;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        // PCはキーボードで視点変更、スマホはジャイロで視点変更
#if UNITY_EDITOR || UNITY_STANDALONE
        float spped = Time.deltaTime * 100.0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotate.y -= spped;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotate.y += spped;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotate.x -= spped;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rotate.x += spped;
        }
        transform.rotation = Quaternion.Euler(rotate);
#else
        Quaternion gattitude = Input.gyro.attitude;
        gattitude.x *= -1;
        gattitude.y *= -1;
        transform.localRotation = Quaternion.Euler(90, -fixrot.y, 0) * gattitude;
#endif

        if (timeCount > 0)
        {
            timeCount -= Time.deltaTime;
            exitButtonOBJ.SetActive(true);
            playButtonOBJ.SetActive(true);
            resetButtonOBJ.SetActive(true);
            modeButtonOBJ.SetActive(true);
        }
        else
        {
            exitButtonOBJ.SetActive(false);
            playButtonOBJ.SetActive(false);
            resetButtonOBJ.SetActive(false);
            modeButtonOBJ.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            timeCount = visibleTimeCount;
        }
    }

    // 再生・一時停止ボタンを押下
    public void OnClickPlayButton()
    {
        playModeFlag = !playModeFlag;
        SetPlayMode(playModeFlag);
    }

    private void SetPlayMode(bool flag)
    {
        if (flag)
        {
            // 一時停止
            videoPlayer.Play();
            playButton.image.sprite = pause_image;
        }
        else
        {
            // 再生
            videoPlayer.Pause();
            playButton.image.sprite = play_image;
        }
    }

    public void OnClickResetButton()
    {
        Debug.Log("Reset");

        nowrot = transform.localEulerAngles;
        fixrot += (nowrot + delrot);  // 修正角度変数に現在の角度を加算
    }

    public void OnClickModeButton()
    {
        Debug.Log("Mode");

        if (modeFlag)
        {
            modeFlag = false;
            modeButton.image.sprite = whiteModeButton_image;
        }
        else
        {
            modeFlag = true;
            modeButton.image.sprite = grayModeButton_image;
        }
    }

    public void SwipeCameraView()
    {
        // タップの種類の判定 & 対応処理
        if ((Input.touchCount == 1 && !isMouseDown) || Input.GetMouseButtonDown(0))
        {
            baseMousePos = Input.mousePosition;
            isMouseDown = true;
        }

        // 指を離した時の処理
        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
        }

        // スワイプ回転処理
        if (isMouseDown)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 distanceMousePos = (mousePos - baseMousePos);
            float angleX = targetObject.transform.eulerAngles.x - distanceMousePos.y * swipeTurnSpeed * 0.01f;
            float angleY = targetObject.transform.eulerAngles.y + distanceMousePos.x * swipeTurnSpeed * 0.01f;

            if ((angleX >= -10f && angleX <= maxCameraAngleX) || (angleX >= minCameraAngleX && angleX <= 370f))
            {
                targetObject.transform.eulerAngles = new Vector3(angleX, angleY, 0);
            }
            else
            {
                targetObject.transform.eulerAngles = new Vector3(targetObject.transform.eulerAngles.x, angleY, 0);
            }
            baseMousePos = mousePos;
        }
    }
}
