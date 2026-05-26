using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class ItemCollect : MonoBehaviour
{
    
    [Header("Conteudo do artefato")]
    public string titulo;

    [TextArea(4, 10)]
    public string descricao;

    [Header("Painel de Informação")]
    public Canvas canvas;
    public TMP_Text textoPainel;

    [Header("Narração")]
    public AudioClip audioNarracao;
    public AudioSource audioSource;
    public bool tocarAudioAoInteragir = true;
    public bool paraAudioAoSoltar = true;

    void Start()
    {
        if (canvas != null) canvas.enabled = false;
        if (audioSource != null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCanvas()
    {
        if (textoPainel != null) textoPainel.text = "<b>" + titulo + "</b>\n\n" + descricao;

        if (canvas != null) canvas.enabled = true;

        if (tocarAudioAoInteragir && audioNarracao != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = audioNarracao;
            audioSource.play();
        }
    }

    public void HideCanvas()
    {
        if (canvas != null) canvas.enabled = false;
        if (paraAudioAoSoltar && audioSource != null && audioSource.isPlaying) audioSource.Stop();
    }
}
