using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace YoutubePlayer
{
    public class OnlineVideoLoader : MonoBehaviour
    {
        YoutubePlayer youtubePlayer;
        VideoPlayer videoPlayer;
        public RenderTexture renderTexture;
        RawImage rawImage;
        public string cachedVideoPath = "CachedVideo.mp4";
        public string url;
        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.url = "";
            rawImage = GetComponent<RawImage>();
            if (rawImage.texture == null)
                rawImage.texture = renderTexture;
        }
        private void OnEnable()
        {
            ClearRenderTexture(Color.black); // �ʱ�ȭ�� ������ ���⿡ �־��ݴϴ�.
            videoPlayer.time = 0;
            if (videoPlayer.url == "")
            {
                youtubePlayer = gameObject.AddComponent<YoutubePlayer>();
                youtubePlayer.youtubeUrl = url;
                youtubePlayer.cli = YoutubePlayer.Cli.YtDlp;
            }
            Prepare(youtubePlayer?.youtubeUrl);
        }

        public async void Prepare(string url = null)
        {
            try
            {
                if (youtubePlayer != null)
                    await youtubePlayer.PrepareVideoAsync();
                StartCoroutine(Play());
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        public IEnumerator Play()
        {
            while (true)
            {
                yield return null;
                if (videoPlayer == null)
                    break;
                //videoPlayer.time = 1;
                //videoPlayer.skipOnDrop = true;
                videoPlayer.Stop();
                videoPlayer.Play();
                break;
            }
        }


        void ClearRenderTexture(Color clearColor)
        {
            // RenderTexture�� ���� Ȱ��ȭ�� RenderTexture�� �����մϴ�.
            RenderTexture.active = renderTexture;

            // clearColor�� RenderTexture�� �ʱ�ȭ�մϴ�.
            GL.Clear(true, true, clearColor);

            // RenderTexture.active�� �ٽ� null�� �����մϴ�.
            RenderTexture.active = null;
        }
        private void OnDisable()
        {
            videoPlayer?.Stop();
            videoPlayer.time = 0;
        }
    }
}
