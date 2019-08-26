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

    public VideoPlayer videoPlayer;
    // true:再生/false:一時停止
    public bool playModeFlag = false;

    public GameObject exitButtonOBJ;
    public GameObject playButtonOBJ;


    public Button PlayButton;
    public Sprite play_image;
    public Sprite pause_image;

    public float visibleTimeCount = 2.0f;
    public float timeCount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // 動作確認用ログ
        Debug.Log("started");

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
        transform.localRotation = Quaternion.Euler(90, 0, 0) * gattitude;
#endif

        if (timeCount > 0)
        {
            timeCount -= Time.deltaTime;
            exitButtonOBJ.SetActive(true);
            playButtonOBJ.SetActive(true);
        }
        else
        {
            exitButtonOBJ.SetActive(false);
            playButtonOBJ.SetActive(false);
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
            PlayButton.image.sprite = pause_image;
        }
        else
        {
            // 再生
            videoPlayer.Pause();
            PlayButton.image.sprite = play_image;
        }
    }
}
