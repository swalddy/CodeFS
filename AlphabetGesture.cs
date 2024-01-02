using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlphabetGesture : MonoBehaviour
{
    public Sprite[] alphabetSprites;
    public Image currentAlphabetImage;
    public TextMeshProUGUI feedbackText;
    public PipeServer2D pipeServer;
    private int currentIndex = 0;
    private int correctCount = 0;

    void Start()
    {
        ShowNextAlphabet();
        pipeServer = PipeServer2D.Instance;
    }

    private void Update()
    {
        string currentGesture = pipeServer.predictedDirection;

        if (CheckGesture(currentGesture))
        {
            correctCount++;  
            feedbackText.text = "Benar!";

            if (correctCount >= 90)  
            {
                correctCount = 0;  
                currentIndex++;  
                if (currentIndex < alphabetSprites.Length)
                {
                    ShowNextAlphabet();
                }
                else
                {
                    feedbackText.text = "Selesai! Kembali ke awal.";
                    currentIndex = 0; 
                    ShowNextAlphabet();
                }
            }
        }
        else
        {
            feedbackText.text = "Salah! Coba lagi.";
        }
    }

    private void ShowNextAlphabet()
    {
        currentAlphabetImage.sprite = alphabetSprites[currentIndex];
    }

    private bool CheckGesture(string gesture)
    {
        return gesture == ((char)('A' + currentIndex)).ToString();
    }

    public string GetCurrentGesture()
    {
        return pipeServer.predictedDirection; 
    }
}
