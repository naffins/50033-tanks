using UnityEngine;
using UnityEngine.UI;

public class InstructionOverlayController : MonoBehaviour {
    
    private const string startGameString = "Start Game!";

    public GameManager gameManager;

    // Array of all instruction overlay screen objects
    public GameObject[] overlays;
    
    // UI buttons to go to previous/next instruction overlay screen
    public GameObject previousButton, nextButton;

    public Text nextButtonText;

    private int currentOverlayIndex = 0;

    private string nextButtonDefaultString;

    private void Awake() {

        // Add listeners to buttons
        previousButton.GetComponent<Button>().onClick.AddListener(()=>{
            this.MoveOverlay(false);
        });
        nextButton.GetComponent<Button>().onClick.AddListener(()=>{
            this.MoveOverlay(true);
        });
        nextButtonDefaultString = nextButtonText.text;

        // Activate the first instruction screen object
        ActivateOverlay(currentOverlayIndex);
    }

    // Move to next/previous instruction overlay screen
    public void MoveOverlay(bool isNextOverlay) {
        if ((isNextOverlay)&&(currentOverlayIndex==overlays.Length-1)) {
            gameManager.StartGame();

            // Disable instruction screen system
            gameObject.SetActive(false);
            return;
        }

        // Disable current instruction screen and update overlay index
        overlays[currentOverlayIndex].SetActive(false);
        currentOverlayIndex += isNextOverlay? 1 : -1;

        ActivateOverlay(currentOverlayIndex);
    }

    private void ActivateOverlay(int currentOverlayIndex) {
        overlays[currentOverlayIndex].SetActive(true);
        previousButton.SetActive(currentOverlayIndex!=0);
        nextButtonText.text = (currentOverlayIndex==overlays.Length-1)? startGameString : nextButtonDefaultString;
    }
    
}