using UnityEngine;
using UnityEngine.UI;

public class SwitchBackground : MonoBehaviour
{
    public static int[] CurrentImageIndex = new int[Game.ScenesCount];

    [SerializeField] private Sprite[] _listBackgroundImages;
    [SerializeField] private GameObject _theBackgroundPrefab;
    [SerializeField] private GameObject _contentBuffer;
    [SerializeField] private GameObject _gridContentBuffer;
    private Button[] _cellsButtons;
    private Button[] _gridCellsButtons;
    private GameObject[] _backgroundCells;
    private GameObject[] _gridBackgroundCells;
    private SelectBackground[] _scripts;
    private SelectBackground[] _gridScripts;
    private const int _heigth = 440;

    private void Awake()
    {
        _backgroundCells = new GameObject[_listBackgroundImages.Length];
        _gridBackgroundCells = new GameObject[_listBackgroundImages.Length];
        _cellsButtons = new Button[_listBackgroundImages.Length];
        _gridCellsButtons = new Button[_listBackgroundImages.Length];
        _scripts = new SelectBackground[_listBackgroundImages.Length];
        _gridScripts = new SelectBackground[_listBackgroundImages.Length];
    }

    private void Start()
    {
        InitialBackgrounds();
        DisplayCurrentBackground();
    }

    public void NextBackground()
    {
        CurrentImageIndex[Game.CurrentScene]++;
        DisplayCurrentBackground();
    }

    public void PreviousBackground()
    {
        CurrentImageIndex[Game.CurrentScene]--;
        DisplayCurrentBackground();
    }

    private void InitialBackgrounds()
    {
        int i = 0;

        foreach (Sprite sprite in _listBackgroundImages)
        {
            _backgroundCells[i] = Instantiate(_theBackgroundPrefab, _contentBuffer.transform);
            _scripts[i] = _backgroundCells[i].GetComponent<SelectBackground>();
            _scripts[i].Init();
            _scripts[i].BackgroundIndex = i;
            _backgroundCells[i].GetComponentInChildren<Image>().sprite = sprite;
            _cellsButtons[i] = _backgroundCells[i].GetComponentInChildren<Button>();
            _cellsButtons[i].onClick.AddListener(HideAllSelectedMessages);
            _cellsButtons[i].onClick.AddListener(_scripts[i].Select);

            _gridBackgroundCells[i] = Instantiate(_theBackgroundPrefab, _gridContentBuffer.transform);
            _gridScripts[i] = _gridBackgroundCells[i].GetComponent<SelectBackground>();
            _gridScripts[i].Init();
            _gridScripts[i].BackgroundIndex = i;
            //_gridBackgroundCells[i] = _gridContentBuffer.transform.GetChild(i).gameObject;
            //if ((i + 1) % 2 == 0)
            //{
            //    Debug.Log("i = " + i);
            //    _gridBackgroundCells[i].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            //    _gridBackgroundCells[i].GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            //    _gridBackgroundCells[i].transform.position = new Vector2(_heigth * i, );
            //}
            //else
            //{
            //    _gridBackgroundCells[i].GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            //    _gridBackgroundCells[i].GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            //}
            _gridBackgroundCells[i].GetComponentInChildren<Image>().sprite = sprite;
            _gridCellsButtons[i] = _gridBackgroundCells[i].GetComponentInChildren<Button>();
            _gridCellsButtons[i].onClick.AddListener(HideAllSelectedMessages);
            _gridCellsButtons[i].onClick.AddListener(_gridScripts[i].Select);

            i++;
        }
    }

    private void DisplayCurrentBackground()
    {
        HideAllSelectedMessages();

        if (CurrentImageIndex[Game.CurrentScene] >= _listBackgroundImages.Length)
            CurrentImageIndex[Game.CurrentScene] = 0;
        if (CurrentImageIndex[Game.CurrentScene] < 0)
            CurrentImageIndex[Game.CurrentScene] = _listBackgroundImages.Length - 1;

        _scripts[CurrentImageIndex[Game.CurrentScene]].Select();
        _gridScripts[CurrentImageIndex[Game.CurrentScene]].Select();
    }

    private void HideAllSelectedMessages()
    {
        for (int i = 0; i < _listBackgroundImages.Length; i++)
        {
            _scripts[i].HideSelected();
            _gridScripts[i].HideSelected();
        }
    }
}