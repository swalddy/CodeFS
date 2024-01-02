using System;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, Interactable
{
    public string nama;
    private int indexHurufSaatIni = 0;
    private Action onCompletion;
    public Image[] placeholderImages;
    public PipeServer2D pipeServer;
    public Image specialImage;
    private bool sudahDiinteraksi = false;
    private float gestureTimer = 0.0f;
    private const float gestureDuration = 1.0f;

    public bool IsInteracted { get { return sudahDiinteraksi; } }

    private void Start()
    {
        DeactivateAllPlaceholders();
        if (specialImage != null)
            specialImage.gameObject.SetActive(false);
        pipeServer = PipeServer2D.Instance;
    }

    private void Update()
    {
        CheckAndActivatePlaceholder();
    }

    public void Interact()
    {
        if (sudahDiinteraksi) return;

        Interact(null); 
    }

    public void Interact(Action onCompletion)
    {
        DeactivateAllPlaceholders();
        if (specialImage != null)
            specialImage.gameObject.SetActive(true);

        this.onCompletion = onCompletion;
        indexHurufSaatIni = 0;
        gestureTimer = 0.0f; 
        if (indexHurufSaatIni < nama.Length)
        {
            ActivatePlaceholderAt(indexHurufSaatIni);
        }
    }

    private void CheckAndActivatePlaceholder()
    {
        string currentGesture = pipeServer.predictedDirection;

        if (!string.IsNullOrEmpty(currentGesture))
        {
            string expectedGesture = nama[indexHurufSaatIni].ToString();

            if (currentGesture.ToUpper() == expectedGesture.ToUpper())
            {
                gestureTimer += Time.deltaTime;

                if (gestureTimer >= gestureDuration)
                {
                    indexHurufSaatIni++;
                    pipeServer.ResetGesture();
                    gestureTimer = 0.0f;

                    if (indexHurufSaatIni < nama.Length)
                    {
                        ActivatePlaceholderAt(indexHurufSaatIni);
                    }
                    else
                    {
                        CompleteSpelling();
                    }
                }
            }
            else
            {
                gestureTimer = 0.0f;
            }
        }
    }

    private void CompleteSpelling()
    {
        DeactivateAllPlaceholders();
        if (specialImage != null)
            specialImage.gameObject.SetActive(false);
        
        onCompletion?.Invoke();
        sudahDiinteraksi = true;
    }

    public void ActivatePlaceholderAt(int index)
    {
        if (index >= 0 && index < placeholderImages.Length)
        {
            placeholderImages[index].gameObject.SetActive(true);
        }
    }

    public void DeactivateAllPlaceholders()
    {
        foreach (var image in placeholderImages)
        {
            if (image != null)
                image.gameObject.SetActive(false);
        }
    }
}
