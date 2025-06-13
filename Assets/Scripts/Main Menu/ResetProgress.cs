using UnityEngine;

public class ResetProgress : MonoBehaviour
{
    public GameObject deleteFileOverlay;
    private const string SAVE_KEY = "LastSavedZone";

    public void ShowOverlay()
    {
        deleteFileOverlay.SetActive(true);
    }

    public void EraseProgress()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save(); // Asegura que los cambios se guarden inmediatamente
        Debug.Log("Save data deleted for " + SAVE_KEY);
        deleteFileOverlay.SetActive(false);

        // Aquí puedes añadir cualquier lógica adicional necesaria después de borrar el progreso
        // Por ejemplo, actualizar la UI, reiniciar variables de juego, etc.
    }

    public void CancelErase()
    {
        deleteFileOverlay.SetActive(false);
    }
}