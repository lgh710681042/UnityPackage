using System.Collections;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace MyCommandLine
{
    public enum ConsoleState
    {
        Client,
        Server
    }
    internal class ServerCmdInfo
    {
        public string name = null;
        public Dictionary<string, string> serverParams = new Dictionary<string, string>();
    }
    internal class ConsoleView : MonoBehaviour
    {
#pragma warning disable 0649
        public RectTransform pmParent; // PMUI
        public RectTransform listView; //PMlistView
        public RectTransform viewPort; //PMlistView -> ViewPort
        public RectTransform content; //PMlistView -> ViewPort -> Content
        public RectTransform item; //PMlistView -> ViewPort -> Content ->Item
        public RectTransform paramView; // ParamListView
        public RectTransform paramBtn; // ParamListView -> TriggerBtn
        public RectTransform paramViewPort; // ParamListView -> ViewPort
        public RectTransform paramParent; // ParamListView -> ViewPort -> Content
        public RectTransform paramItem; // ParamListView -> ViewPort -> Content -> Item
        public Image img_Bg; 
        public Text showTipsText;
        public InputField inputControl; 
        public RectTransform showOrHide; 
        public RectTransform serverView; 
        public RectTransform clientView; 
        public RectTransform clientBtn; 
        public RectTransform serverBtn; 
        public RectTransform sendBtn;
        public RectTransform closeBtn;
        public RectTransform consoleView;
        public RectTransform consoleViewPort;
        public RectTransform consoleViewContent;
        public RectTransform consoleViewItem;

        public RectTransform historyConsoleView;
        public RectTransform historyConsoleViewPort;
        public RectTransform historyConsoleViewContent;
        public RectTransform historyConsoleViewItem;

        public RectTransform hideConsole;
        public Text serverInfo;
#pragma warning restore 0649
        private Text showTips;

        private List<GameObject> itemList = new List<GameObject>(); // 所有Item
        private List<GameObject> paramItemList = new List<GameObject>(); // PM命令参数paramItem
        private List<GameObject> consoleList = new List<GameObject>();
        private int saveCount = 10;
        private List<ServerCmdInfo> serverCmdInfos = new List<ServerCmdInfo>();
        
        private int curSelectIndex = -1;
        private ConsoleManager m_consoleMgr;

        void Start()
        {
            if (ConsolePM.GetInstance().ConsoleMgr == null) return;
            m_consoleMgr = ConsolePM.GetInstance().ConsoleMgr;
            InitUISize();
            //InitItems();
            RefreshConsoleItems("");
            showTips = showOrHide.GetComponentInChildren<Text>();
            inputControl.onValueChanged.AddListener(OnInput);
        }
        
        public bool ShowOrHideView(bool isShow)
        {
            gameObject.SetActive(isShow);
            if (!isShow)
            {
                consoleView.gameObject.SetActive(false);
                historyConsoleView.gameObject.SetActive(false);
                //hideConsole.gameObject.SetActive(false);
                paramBtn.gameObject.SetActive(false);
                paramView.gameObject.SetActive(false);
            }
            else
            {
                consoleView.gameObject.SetActive(true);
                historyConsoleView.gameObject.SetActive(true);
            }

            showTipsText.text = isShow ? "Hide PM" : "Show PM";
            img_Bg.enabled = isShow;
            return isShow;
        }
        
        // init ui
        private void InitUISize()
        {
            var inputControlTransform = inputControl.GetComponent<RectTransform>();
            pmParent.offsetMin = Vector2.zero;
            pmParent.offsetMax = Vector2.zero;
            clientView.offsetMax = Vector2.zero;
            clientView.offsetMin = Vector2.zero;
            var height = pmParent.rect.height;
            var width = pmParent.rect.width / 3;
            float space = 10;
            inputControlTransform.sizeDelta = new Vector2(width, height / 18);
            inputControlTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, space, inputControlTransform.rect.height);
            inputControlTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, space, inputControlTransform.rect.width);

            sendBtn.sizeDelta = new Vector2(width / 5, height / 18);
            sendBtn.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, space, sendBtn.rect.height);
            sendBtn.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, space + inputControlTransform.rect.width, sendBtn.rect.width);

            var closeBtnHOffset = space + inputControlTransform.rect.width + sendBtn.rect.width;

            closeBtn.sizeDelta = new Vector2(width / 5, height / 18);
            closeBtn.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, space, closeBtn.rect.height);
            closeBtn.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, closeBtnHOffset, closeBtn.rect.width); 
           
            var consoleInset = space + inputControlTransform.rect.height;
            consoleView.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, consoleInset, height * 2 / 3);
            consoleView.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, clientBtn.rect.width + space, width + space * 2);

            consoleViewPort.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, consoleInset, height * 2 / 3);
            consoleViewPort.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, width + space, width + space * 2);
            consoleViewPort.anchoredPosition = Vector2.zero;

            consoleViewItem.sizeDelta = new Vector2(consoleViewPort.sizeDelta.x, height / 15);

            var consoleHeight = consoleInset + consoleView.rect.height;
            var historyConsoleHeight = height - consoleHeight;

            historyConsoleView.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, consoleHeight, historyConsoleHeight);
            historyConsoleView.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, clientBtn.rect.width + space, width + space * 2);

            historyConsoleViewPort.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, consoleHeight, historyConsoleHeight);
            historyConsoleViewPort.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, width + space, width + space * 2);
            historyConsoleViewPort.anchoredPosition = Vector2.zero;

            historyConsoleViewItem.sizeDelta = new Vector2(consoleViewPort.sizeDelta.x, height / 15);

            listView.gameObject.SetActive(false);
            viewPort.gameObject.SetActive(false);
            clientBtn.gameObject.SetActive(false);
            serverBtn.gameObject.SetActive(false);
            hideConsole.gameObject.SetActive(false);

            sendBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                SubmitInput();
            });

            closeBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                ConsolePM.GetInstance().HideView();
            });
        }

        private void InitHistoryConsoleItems()
        {
            var list = GetCacheList();
            var reg = new Regex(@"(\w+)\s");

            for (var i = list.Count - 1; i >= 0; i--)
            {
                var obj = GetItem(consoleList, historyConsoleViewItem.gameObject, historyConsoleViewContent);
                obj.SetActive(true);
                var texts = obj.GetComponentsInChildren<Text>();
                var name = reg.Match(list[i]).Groups[1].Value;
                texts[0].text = name;
                texts[1].text = list[i];
                obj.GetComponent<Image>().color = new Color(0, 0, 1, 0f);
                var btn = obj.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    inputControl.text = texts[1].text;
                    inputControl.ActivateInputField();
                    StartCoroutine(MoveNext());
                });
            }
        }

        private void InitConsoleItems()
        {
            var data = m_consoleMgr.ConsoleData();
            
            foreach (var item in data.pmInfos)
            {
                var obj = GetItem(consoleList, consoleViewItem.gameObject, consoleViewContent);
                obj.SetActive(true);
                var texts = obj.GetComponentsInChildren<Text>();
                texts[0].text = item.bute.Name;
                texts[1].text = item.bute.Description;
                obj.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                var btn = obj.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    inputControl.text = texts[0].text + " ";
                    inputControl.ActivateInputField();
                    StartCoroutine(MoveNext());
                });
            }
        }

        private IEnumerator MoveNext()
        {
            yield return 0;
            inputControl.MoveTextEnd(false);
        }

        private void RefreshConsoleItems(string content)
        {
            HideAllItem(consoleList);
            if (content.Length <= 0)
            {
                InitConsoleItems();
                InitHistoryConsoleItems();
                return;
            }
            var datas = m_consoleMgr.ConsoleData().pmInfos;
            foreach (var itemData in datas)
            {
                var head = GetHead(content);
                if (head.Length > 0)
                {
                    head = head.Substring(0, head.Length - 1);
                    if (itemData.bute.Name.Equals(head, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var paramData in itemData.bute.ParamArray)
                        {
                            var obj = GetItem(consoleList, consoleViewItem.gameObject, consoleViewContent);
                            obj.SetActive(true);
                            var texts = obj.GetComponentsInChildren<Text>();
                            texts[0].text = paramData.Name;
                            texts[1].text = paramData.Description;
                            obj.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                            var btn = obj.GetComponent<Button>();
                            btn.onClick.RemoveAllListeners();
                        }
                    }
                }
                else if (content.Length <= itemData.bute.Name.Length)
                {
                    head = itemData.bute.Name.Substring(0, content.Length);
                    if (head.Equals(content, StringComparison.OrdinalIgnoreCase))
                    {
                        var obj = GetItem(consoleList, consoleViewItem.gameObject, consoleViewContent);
                        obj.SetActive(true);
                        var texts = obj.GetComponentsInChildren<Text>();
                        texts[0].text = itemData.bute.Name;
                        texts[1].text = itemData.bute.Description;
                        obj.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                        var btn = obj.GetComponent<Button>();
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() =>
                        {
                            inputControl.text = texts[0].text + " ";
                            inputControl.ActivateInputField();
                            StartCoroutine(MoveNext());
                        });
                    }
                }
            }
            RefreshSelectItem();
        }

        private void RefreshSelectItem()
        {
            for (int i = 0; i < consoleList.Count; i++)
            {
                if (consoleList[i].activeSelf)
                {
                    if (curSelectIndex > 0)
                    {
                        consoleList[curSelectIndex].GetComponent<Image>().color = new Color(0, 0, 1, 0f);
                    }
                    curSelectIndex = i;
                    consoleList[curSelectIndex].GetComponent<Image>().color = new Color(0, 0, 1, 0.3f);
                    break;
                }
            }
        }

        private string GetHead(string info)
        {
            Regex reg = new Regex(@"[a-zA-Z0-9]+\s", RegexOptions.IgnoreCase);
            var math = reg.Match(info);
            return math.Value;
        }
        // hide all ui item
        private void HideAllItem(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetActive(false);
            }
        }

        private GameObject GetItem(List<GameObject> list, GameObject itemObj, RectTransform parent)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].activeSelf)
                {
                    return list[i];
                }
            }
            var obj = Instantiate<GameObject>(itemObj.gameObject);
            obj.transform.SetParent(parent, false);
            list.Add(obj);
            return obj;
        }
        // search
        public void OnInput(string name)
        {
            if (!consoleView.gameObject.activeSelf)
            {
                consoleView.gameObject.SetActive(true);
            }
            //if (!hideConsole.gameObject.activeSelf)
            //{
            //    hideConsole.gameObject.SetActive(true);
            //}
            if (paramView.gameObject.activeSelf)
            {
                paramView.gameObject.SetActive(false);
            }

            RefreshConsoleItems(name);
        }
        
        private void SubmitInput()
        {
            try
            {
                m_consoleMgr.TriggerPm(inputControl.text);
            }
            catch (Exception e)
            {
                Debug.Log(" Cmd Execute Failed " + e.Message);
                RefreshConsoleItems("");
                inputControl.text = "";
                return;
            }

            Debug.Log(" Cmd Execute Success ");
            string content = inputControl.text;
            if (content != null && !content.Contains("DeleteConsoleCache"))
            {
                SaveCache(content);
            }
            RefreshConsoleItems("");
            inputControl.text = "";
        }

        private void SaveCache(string content)
        {
            var list = new List<string>();
            for (var i = 0; i < saveCount; i++)
            {
                var key = "PMModule_" + i;
                if (PlayerPrefs.HasKey(key))
                {
                    list.Add(PlayerPrefs.GetString(key));
                }
            }
            if (list.Contains(content))
            {
                return;
            }
            if (list.Count == saveCount)
            {
                list.RemoveAt(0);
            }
            list.Add(content);
            for (var i = 0; i < list.Count; i++)
            {
                var key = "PMModule_" + i;
                PlayerPrefs.SetString(key, list[i]);
            }
        }
        private List<string> GetCacheList()
        {
            var list = new List<string>();
            for (var i = 0; i < saveCount; i++)
            {
                var key = "PMModule_" + i;
                if (PlayerPrefs.HasKey(key))
                {
                    list.Add(PlayerPrefs.GetString(key));
                }
            }
            return list;
        }

        public void DeleteCache()
        {
            for (var i = 0; i < saveCount; i++)
            {
                var key = "PMModule_" + i;
                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
            RefreshConsoleItems("");
        }
    }
}
