using System.Collections.Generic;
using UnityEngine.Events;

namespace Overlayer.Utils
{
    public static class ADOUtils
    {
        static ErrorCanvas overlayerErrorCanvas;
        public static void ShowError(ErrorCanvasContext ecc)
        {
            if (overlayerErrorCanvas == null)
            {
                var ecObject = UnityEngine.Object.Instantiate(RDConstants.data.prefab_errorCanvas);
                var ec = ecObject.GetComponent<ErrorCanvas>();
                UnityEngine.Object.DontDestroyOnLoad(ecObject);
                overlayerErrorCanvas = ec;
            }
            overlayerErrorCanvas.btnSupport.onClick.RemoveAllListeners();
            overlayerErrorCanvas.btnLog.onClick.RemoveAllListeners();
            overlayerErrorCanvas.btnSubmit.onClick.RemoveAllListeners();
            overlayerErrorCanvas.btnIgnore.onClick.RemoveAllListeners();
            overlayerErrorCanvas.btnBack.onClick.RemoveAllListeners();
            if (ecc.supportBtnCallback != null)
                overlayerErrorCanvas.btnSupport.onClick.AddListener(ecc.supportBtnCallback);
            if (ecc.logBtnCallback != null)
                overlayerErrorCanvas.btnLog.onClick.AddListener(ecc.logBtnCallback);
            if (ecc.submitBtnCallback != null)
                overlayerErrorCanvas.btnSubmit.onClick.AddListener(ecc.submitBtnCallback);
            if (ecc.ignoreBtnCallback != null)
                overlayerErrorCanvas.btnIgnore.onClick.AddListener(ecc.ignoreBtnCallback);
            if (ecc.goBackBtnCallback != null)
                overlayerErrorCanvas.btnBack.onClick.AddListener(ecc.goBackBtnCallback);
            overlayerErrorCanvas.btnSupport.gameObject.SetActive(ecc.supportBtnCallback != null);
            overlayerErrorCanvas.btnLog.gameObject.SetActive(ecc.logBtnCallback != null);
            overlayerErrorCanvas.btnSubmit.gameObject.SetActive(ecc.submitBtnCallback != null);
            overlayerErrorCanvas.btnIgnore.gameObject.SetActive(ecc.ignoreBtnCallback != null);
            overlayerErrorCanvas.btnBack.gameObject.SetActive(ecc.goBackBtnCallback != null);
            overlayerErrorCanvas.txtTitle.text = ecc.titleText ?? RDString.Get("error.somethingWentWrong");
            overlayerErrorCanvas.txtSubmit.text = ecc.submitText ?? RDString.Get("error.submit");
            overlayerErrorCanvas.txtSupportPages.text = ecc.supportPagesText ?? RDString.Get("error.supportPages");
            overlayerErrorCanvas.txtFaq.text = ecc.faqText ?? RDString.Get("error.faq");
            overlayerErrorCanvas.txtDiscord.text = ecc.discordText ?? RDString.Get("error.discord");
            overlayerErrorCanvas.txtSteam.text = ecc.steamText ?? RDString.Get("error.steam");
            overlayerErrorCanvas.txtGoBack.text = ecc.goBackText ?? RDString.Get("error.goBack");
            overlayerErrorCanvas.txtErrorMessage.text = ecc.errorMessage;
            overlayerErrorCanvas.gameObject.SetActive(true);
        }
        public static void HideError(ErrorCanvasContext ecc)
        {
            overlayerErrorCanvas.gameObject.SetActive(false);
        }
        public static int HashMargins(HitMargin[] margins)
        {
            int bits = 0;
            for (int i = 0; i < margins.Length; i++)
                bits |= 1 << (int)margins[i];
            return bits;
        }
        public static HitMargin[] UnboxMarginHash(int marginHash)
        {
            List<HitMargin> margins = new List<HitMargin>();
            var values = EnumHelper<HitMargin>.GetValues();
            for (int i = 0; i < values.Length; i++)
            {
                if ((marginHash & (1 << (int)values[i])) != 0)
                    margins.Add(values[i]);
            }
            return margins.ToArray();
        }
    }
    public class ErrorCanvasContext
    {
        /// <summary>
        /// StackTrace Or Message?
        /// </summary>
        public string errorMessage;

        public string titleText;
        public string submitText;
        public string supportPagesText;
        public string faqText;
        public string discordText;
        public string steamText;
        public string goBackText;

        public UnityAction supportBtnCallback;
        public UnityAction logBtnCallback;
        public UnityAction submitBtnCallback;
        public UnityAction ignoreBtnCallback;
        public UnityAction goBackBtnCallback;
    }
}
