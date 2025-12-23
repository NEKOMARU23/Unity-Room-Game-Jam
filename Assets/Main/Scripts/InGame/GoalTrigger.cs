using UnityEngine;
using System.Collections;
public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private string goalSeName = "GoalSE";
    [SerializeField] private float waitSeconds = 0.2f;
    private bool cleared = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cleared) return;
        if (!other.CompareTag("Player")) return;

        cleared = true;
        StartCoroutine(PlaySeAndLoad());
    }

    private IEnumerator PlaySeAndLoad()
    {
        //     // ゴールSE
        //     if (AudioManager.Instance != null)
        //         AudioManager.Instance.Play(goalSeName);

        // SEが聞こえるように少し待つ
        yield return new WaitForSeconds(waitSeconds);

             // リザルトへ
        //     if (SceneController.Instance != null)
        //         SceneController.Instance.LoadScene(SceneName.Clear);
        Debug.Log("ゴールしました！");
    }
}
