using UnityEngine;
using TMPro;

public class InfoPanelManager : MonoBehaviour
{
    public static InfoPanelManager Instance;

    [Header("Painel")]
    public Canvas canvas;
    public TMP_Text textoPainel;

    [Header("Seguir objeto")]
    public Vector3 offset = new Vector3(0.35f, 0.25f, 0f);
    public bool seguirObjeto = true;

    [Header("Billboard")]
    public Camera playerCamera;

    [Header("Texto")]
    public float tamanhoFonte = 28f;
    public bool limitarTexto = true;

    [Header("Áudio opcional")]
    public AudioSource audioSource;

    private Transform alvoAtual;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (textoPainel != null)
        {
            textoPainel.enableWordWrapping = true;
            textoPainel.fontSize = tamanhoFonte;

            if (limitarTexto)
            {
                textoPainel.overflowMode = TextOverflowModes.Ellipsis;
            }
        }

        Hide();
    }

    private void LateUpdate()
    {
        if (canvas != null && canvas.enabled)
        {
            if (seguirObjeto && alvoAtual != null)
            {
                canvas.transform.position = alvoAtual.position + offset;
            }

            FacePlayer();
        }
    }

    public void Show(string titulo, string descricao, AudioClip audioClip, Transform target)
    {
        alvoAtual = target;

        if (textoPainel != null)
        {
            textoPainel.text = "<b>" + titulo + "</b>\n\n" + descricao;
        }

        if (canvas != null)
        {
            if (alvoAtual != null)
            {
                canvas.transform.position = alvoAtual.position + offset;
            }

            canvas.enabled = true;
            FacePlayer();
        }

        if (audioSource != null)
        {
            audioSource.Stop();

            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
    }

    public void Hide()
    {
        alvoAtual = null;

        if (canvas != null)
        {
            canvas.enabled = false;
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void FacePlayer()
    {
        if (canvas == null || playerCamera == null)
        {
            return;
        }

        Vector3 direction = canvas.transform.position - playerCamera.transform.position;
        canvas.transform.rotation = Quaternion.LookRotation(direction);
    }
}