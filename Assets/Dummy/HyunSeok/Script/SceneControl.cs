﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControl : MonoBehaviour
{
    public static SceneControl _instance;
    public Text loadingText;

    public List<string> scripts;

    #region private field
    [SerializeField]
    GameObject loadingPanel;
    Animator loadingAnimator;
    Coroutine loadingCrtn;
    #endregion
    #region unity method
    void Awake ()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);
        loadingPanel.SetActive(false);
        DontDestroyOnLoad (this.gameObject);
        // 컴포넌트 할당
        loadingAnimator = loadingPanel.GetComponent<Animator> ();
    }
    /**
     *   지정한 씬을 비동기 로드한다.
     *   @param sceneName    씬 이름
     */

    public void LoadTargetScene(string sceneName)
    {
        if (loadingCrtn == null)
            loadingCrtn = StartCoroutine(LoadScene(sceneName));
    }
    #endregion
    #region custom method
    /**
     *   지정된 씬을 로드
     *   @param sceneName        로드할 씬 이름
     */
    public IEnumerator LoadScene (string sceneName)
    {
        // 로딩 패널 켜기
        if (loadingPanel != null)
            loadingPanel.SetActive (true);
        int rand = Random.Range(0, scripts.Count);
        loadingText.text = scripts[rand];
        // 비동기 작업 지정
        AsyncOperation operation = SceneManager.LoadSceneAsync (sceneName);

        // 씬이 로딩이 완료 될 때까지 비활성화
        operation.allowSceneActivation = false;
        float time = 0;
        while (!operation.isDone)
        {
            time += Time.deltaTime;
            // 만약 로딩이 완료됬을 경우 씬을 활성화 한다
            if (operation.progress >= 0.9f && time >= 1.0f)
            {
                operation.allowSceneActivation = true;
                loadingAnimator.SetTrigger ("TrgLoaded");
                break;
            }
            yield return null;
        }
        loadingCrtn = null;
    }
    #endregion
}
