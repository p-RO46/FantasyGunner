using LLMUnity;
using TMPro;
using UnityEngine;

public class SupplierTalk : MonoBehaviour
{
    public PlayerInventory pi;
    public TextMeshProUGUI speech;
    public TMP_InputField input;
    public LLMAgent llmAgent;

    void Start()
    {
        // 코드에서 리스너를 등록하는 방법 (인스펙터에서 연결하는 것보다 관리가 쉽습니다)
        input.onSubmit.AddListener(SubmitText);
    }

    void HandleReply(string replySoFar)
    {
        speech.text = replySoFar;
        // do something with the reply from the model as it is being produced
        //Debug.Log(replySoFar);
    }

    void SubmitText(string text)
    {
        // 1. 공백만 입력되었는지 확인
        if (string.IsNullOrWhiteSpace(text)) return;

        // 2. 텍스트 전달 (여기서는 로그를 찍지만, 보급병 AI 시스템으로 넘기시면 됩니다)
        // 현재 데이터 기반으로 상황 정보(Context) 생성
        string context = $"[Current Player Status] Scrap: {pi.GetScrap()}\n";

        // 전체 프롬프트 구성 (시스템 설정 + 현재 데이터 + 플레이어 질문)
        string fullPrompt = context + "Player asks: " + text;
        _ = llmAgent.Chat(text, HandleReply);

        // 3. 입력창 비우기
        input.text = "";

        // 4. (선택 사항) 전송 후 바로 다시 입력할 수 있도록 포커스 유지
        input.ActivateInputField();
    }
}
