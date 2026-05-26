using UnityEngine;

public class ItemCollect : MonoBehaviour
{
    [Header("Conteúdo do Artefato")]
    public string titulo = "Título do Artefato";

    [TextArea(4, 10)]
    public string descricao = "Descrição histórica do artefato.";

    [Header("Áudio Opcional")]
    public AudioClip audioNarracao;

    [Header("Objeto visual que será reduzido")]
    public Transform objetoVisual;

    [Header("Escala ao pegar")]
    public bool reduzirAoPegar = true;

    [Tooltip("0.5 = metade do tamanho original. 0.25 = um quarto do tamanho original.")]
    public float multiplicadorEscalaAoPegar = 0.45f;

    private Vector3 escalaOriginal;

    private void Awake()
    {
        if (objetoVisual != null)
        {
            escalaOriginal = objetoVisual.localScale;
        }
    }

    public void ShowCanvas()
    {
        if (InfoPanelManager.Instance != null)
        {
            InfoPanelManager.Instance.Show(titulo, descricao, audioNarracao, transform);
        }

        if (reduzirAoPegar && objetoVisual != null)
        {
            objetoVisual.localScale = escalaOriginal * multiplicadorEscalaAoPegar;
        }
    }

    public void HideCanvas()
    {
        if (InfoPanelManager.Instance != null)
        {
            InfoPanelManager.Instance.Hide();
        }

        if (reduzirAoPegar && objetoVisual != null)
        {
            objetoVisual.localScale = escalaOriginal;
        }
    }
}