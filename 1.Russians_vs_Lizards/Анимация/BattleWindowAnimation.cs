using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindowAnimation : MonoBehaviour
{
    [SerializeField] private Animator _battleButton;
    [SerializeField] private Animator _skills;  
    [SerializeField] private Animator _hero;  
    [SerializeField] private Animator _inventory;
    [SerializeField] private Animator _stageMenu;
    [SerializeField] private Animator _nextStageButton;
    [SerializeField] private Animator _battleExitButton;
    [SerializeField] private Animator _averageExitButton;
    [SerializeField] private Animator _arsenalMiniButton;
    [SerializeField] private Animator _skillsMiniButton;
    [SerializeField] private Animator _perkTreeMiniButton;
    [SerializeField] private Animator _arsenalButton;
    [SerializeField] private Animator _skillsButton;
    [SerializeField] private Animator _perkTreeButton;
    [SerializeField] private Animator _shopButton;
    [SerializeField] private Animator _resetProgressButton;
    [SerializeField] private Animator _multiverseButton;
    private Animator _battleWindow;
    private bool _open = false;

    private void Start()
    {
        _battleWindow = gameObject.GetComponent<Animator>();
    }

    public void SwitchBattleWindow()
    {
        if (!_open) StartCoroutine(PeriodicEvent("Open", 0.1f));
        else StartCoroutine(PeriodicEvent("Close", 0.1f));

        _open = !_open;

        IEnumerator PeriodicEvent(string state, float time)
        {
            if (state == "Open")
            {
                _averageExitButton.gameObject.GetComponent<Button>().interactable = false;
                _battleButton.gameObject.GetComponent<Button>().interactable = false;
                _battleButton.SetTrigger("Close");

                _perkTreeButton.SetTrigger("Close");  
                yield return new WaitForSeconds(time);

                _skillsButton.SetTrigger("Close");
                yield return new WaitForSeconds(time);

                _arsenalButton.SetTrigger("Close");
                yield return new WaitForSeconds(time);

                _shopButton.SetTrigger("Close");
                yield return new WaitForSeconds(time);

                _resetProgressButton.SetTrigger("Close");
                yield return new WaitForSeconds(time);

                _multiverseButton.SetTrigger("Close");
                yield return new WaitForSeconds(time);

                _skills.gameObject.SetActive(true);
                _skills.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _hero.gameObject.SetActive(true);
                _hero.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _inventory.gameObject.SetActive(true);
                _inventory.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _perkTreeMiniButton.gameObject.SetActive(true);
                _perkTreeMiniButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _skillsMiniButton.gameObject.SetActive(true);
                _skillsMiniButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _arsenalMiniButton.gameObject.SetActive(true);
                _arsenalMiniButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _stageMenu.gameObject.SetActive(true);
                _stageMenu.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _nextStageButton.gameObject.SetActive(true);
                _nextStageButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _averageExitButton.gameObject.SetActive(true);
                _averageExitButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _averageExitButton.gameObject.GetComponent<Button>().interactable = true;
                _nextStageButton.gameObject.GetComponentInChildren<Button>().interactable = true;
            }

            else
            {
                _averageExitButton.GetComponent<Button>().interactable = false;
                _battleExitButton.GetComponent<Button>().interactable = false;
                _battleButton.gameObject.GetComponent<Button>().interactable = false;

                _battleExitButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _averageExitButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _skills.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _hero.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _inventory.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _perkTreeMiniButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _skillsMiniButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _arsenalMiniButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _perkTreeButton.gameObject.SetActive(true);
                _perkTreeButton.SetTrigger("Open");
                yield return new WaitForSeconds(time);

                _skillsButton.gameObject.SetActive(true);
                _skillsButton.SetTrigger("Open");
                yield return new WaitForSeconds(time);

                _arsenalButton.gameObject.SetActive(true);
                _arsenalButton.SetTrigger("Open");
                yield return new WaitForSeconds(time);

                _shopButton.gameObject.SetActive(true);
                _shopButton.SetTrigger("Open");
                yield return new WaitForSeconds(time);

                _resetProgressButton.gameObject.SetActive(true);
                _resetProgressButton.SetTrigger("Open");
                yield return new WaitForSeconds(time);

                _multiverseButton.gameObject.SetActive(true);
                _multiverseButton.SetTrigger("Open");
                yield return new WaitForSeconds(time);

                _stageMenu.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _nextStageButton.SetTrigger(state);
                yield return new WaitForSeconds(time);

                _battleButton.gameObject.SetActive(true);
                _battleButton.SetTrigger("Open");
                yield return new WaitForSeconds(time);

                _battleWindow.SetTrigger(state);
            }
        }
    }

    public void SetInteractable()
    {
        _battleButton.GetComponent<Button>().interactable = true;
        _averageExitButton.GetComponent<Button>().interactable = true;
        _battleExitButton.GetComponent<Button>().interactable = true;
    }
}
