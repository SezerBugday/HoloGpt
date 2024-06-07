using System.Threading;
using OpenAI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
      //  public Animator recoderButtonAnim;
     
      public  ChatCPT chatCPT;
        [SerializeField] private Button recordButton;
       // [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text message;
        //[SerializeField] private Dropdown dropdown;
        public AudioSource audioSource;
        private readonly string fileName = "output.wav";
        private readonly int duration = 30;
        public AudioClip start_record,end_record;
        private AudioClip clip;
        public bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi();

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
           // chatCPT = GetComponent<ChatCPT>();
            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            #else
            foreach (var device in Microphone.devices)
            {
               // dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            //dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            //dropdown.SetValueWithoutNotify(index);
            #endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        private void StartRecording()
        {
            if (!isRecording)
            {
               // recoderButtonAnim.SetBool("Recoder",true);
                audioSource.PlayOneShot(start_record);
                isRecording = true;
                //recordButton.enabled = false;

                var index = PlayerPrefs.GetInt("user-mic-device-index");
            
#if !UNITY_WEBGL
                clip = Microphone.Start(Microphone.devices[0], false, duration, 44100);

            
#endif
            }
            else
            {
              
                EndRecording();
                
             
            }

        }

        private async void EndRecording()
        {
            if (isRecording)
            {
                isRecording = false;
              //  recoderButtonAnim.SetBool("Recoder",false);
                audioSource.PlayOneShot(end_record);
            
                Microphone.End(Microphone.devices[0]);
                message.text = "Transcripting...";
            
#if !UNITY_WEBGL
                Microphone.End(null);
#endif
            
                byte[] data = SaveWav.Save(fileName, clip);
            
                var req = new CreateAudioTranscriptionsRequest
                {
                    FileData = new FileData() {Data = data, Name = "audio.wav"},
                    // File = Application.persistentDataPath + "/" + fileName,
                    Model = "whisper-1",
                    Language = "tr"
                };
                var res = await openai.CreateAudioTranscription(req);
                Debug.Log(res.Text);
            
            
                chatCPT.Basla(res.Text);
                isRecording = false;

                // progressBar.fillAmount = 0;
                message.text = res.Text;
                //recordButton.enabled = true;
            }
            

        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
              //  progressBar.fillAmount = time / duration;
                
                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }
}
