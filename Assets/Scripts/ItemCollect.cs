using UnityEngine;
using TMPro;
using System.Collections;

public class ItemCollect : MonoBehaviour
{
    [Header("Conteúdo do Artefato")]
    public string titulo = "Título do Artefato";

    [TextArea(4, 15)]
    public string descricao = "Descrição histórica do artefato.";

    [Header("Referências do painel local")]
    public Canvas canvas;
    public TMP_Text textoDescricao;

    [Header("Posicionamento automático do painel")]
    public bool ajustarPainelAutomaticamente = true;
    public Transform objetoVisual;
    public float ladoDoPainel = 1f;
    public float margemRelativa = 0.15f;
    public float alturaRelativa = 0.65f;
    public float margemMinima = 0.05f;

    [Header("Escala ao pegar")]
    public bool reduzirVisualAoPegar = true;
    public float multiplicadorEscalaAoPegar = 0.45f;

    [Header("Auto Scroll do Texto")]
    public bool usarAutoScroll = true;
    public float atrasoAntesDoScroll = 1.2f;
    public float velocidadeScroll = 25f;
    public RectTransform areaVisivelDoTexto;

    [Header("Padding do Auto Scroll")]
    public float paddingTopoMinimo = 8f;
    public float paddingFinalMinimo = 14f;
    public float paddingRelativo = 0.05f;

    private RectTransform textoRect;
    private Vector2 posicaoInicialTexto;
    private float alturaMaximaScroll;
    private float tempoQueAbriu;
    private bool painelAberto;

    [Header("Áudio opcional")]
    public AudioClip audioNarracao;
    public AudioSource audioSource;
    public bool tocarAudioAoPegar = true;
    public bool pararAudioAoSoltar = true;

    private Vector3 escalaOriginalVisual;
    private Coroutine rotinaPrepararScroll;

    private void Awake()
    {
        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>(true);

        if (textoDescricao == null && canvas != null)
            textoDescricao = canvas.GetComponentInChildren<TMP_Text>(true);

        if (textoDescricao != null)
            textoRect = textoDescricao.GetComponent<RectTransform>();

        if (areaVisivelDoTexto == null && textoDescricao != null)
            areaVisivelDoTexto = textoDescricao.transform.parent.GetComponent<RectTransform>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        if (objetoVisual != null)
            escalaOriginalVisual = objetoVisual.localScale;
    }

    private void Start()
    {
        HideCanvas();
    }

    private void Update()
    {
        if (!painelAberto || !usarAutoScroll || textoRect == null)
            return;

        float tempoDecorrido = Time.time - tempoQueAbriu;

        if (tempoDecorrido < atrasoAntesDoScroll)
            return;

        float scrollAtual = (tempoDecorrido - atrasoAntesDoScroll) * velocidadeScroll;
        scrollAtual = Mathf.Clamp(scrollAtual, 0f, alturaMaximaScroll);

        textoRect.anchoredPosition = posicaoInicialTexto + new Vector2(0f, scrollAtual);
    }

    public void ShowCanvas()
    {
        if (reduzirVisualAoPegar && objetoVisual != null)
            objetoVisual.localScale = escalaOriginalVisual * multiplicadorEscalaAoPegar;

        if (ajustarPainelAutomaticamente)
            AjustarPosicaoDoPainel();

        if (textoDescricao != null)
        {
            textoDescricao.text = descricao;
            textoDescricao.enableWordWrapping = true;
            textoDescricao.overflowMode = TextOverflowModes.Overflow;
            textoDescricao.ForceMeshUpdate();
        }

        if (canvas != null)
            canvas.enabled = true;

        if (rotinaPrepararScroll != null)
            StopCoroutine(rotinaPrepararScroll);

        rotinaPrepararScroll = StartCoroutine(PrepararAutoScrollProximoFrame());

        if (tocarAudioAoPegar && audioNarracao != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = audioNarracao;
            audioSource.Play();
        }
    }

    private IEnumerator PrepararAutoScrollProximoFrame()
    {
        yield return new WaitForEndOfFrame();
        PrepararAutoScroll();
    }

    public void HideCanvas()
    {
        painelAberto = false;

        if (rotinaPrepararScroll != null)
        {
            StopCoroutine(rotinaPrepararScroll);
            rotinaPrepararScroll = null;
        }

        if (canvas != null)
            canvas.enabled = false;

        if (textoRect != null)
            textoRect.anchoredPosition = posicaoInicialTexto;

        if (reduzirVisualAoPegar && objetoVisual != null)
            objetoVisual.localScale = escalaOriginalVisual;

        if (pararAudioAoSoltar && audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void AjustarPosicaoDoPainel()
    {
        if (canvas == null || objetoVisual == null)
            return;

        Renderer[] renderers = objetoVisual.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        Vector3 direcaoLateral = transform.right * Mathf.Sign(ladoDoPainel);

        float tamanhoObjetoNaDirecao = CalcularTamanhoNaDirecao(bounds, direcaoLateral);
        float metadeTamanhoObjeto = tamanhoObjetoNaDirecao * 0.5f;

        float maiorDimensao = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float margemAutomatica = Mathf.Max(margemMinima, maiorDimensao * margemRelativa);

        float metadeLarguraCanvas = CalcularMetadeLarguraCanvas();
        float alturaAutomatica = bounds.extents.y * alturaRelativa;

        Vector3 posicaoPainel =
            bounds.center +
            direcaoLateral * (metadeTamanhoObjeto + metadeLarguraCanvas + margemAutomatica) +
            Vector3.up * alturaAutomatica;

        canvas.transform.position = posicaoPainel;
    }

    private float CalcularMetadeLarguraCanvas()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        if (canvasRect == null)
            return 0f;

        Vector3[] corners = new Vector3[4];
        canvasRect.GetWorldCorners(corners);

        float larguraMundo = Vector3.Distance(corners[0], corners[3]);

        return larguraMundo * 0.5f;
    }

    private float CalcularTamanhoNaDirecao(Bounds bounds, Vector3 direcao)
    {
        direcao.Normalize();

        Vector3 centro = bounds.center;
        Vector3 ext = bounds.extents;

        Vector3[] pontos = new Vector3[8]
        {
            centro + new Vector3( ext.x,  ext.y,  ext.z),
            centro + new Vector3( ext.x,  ext.y, -ext.z),
            centro + new Vector3( ext.x, -ext.y,  ext.z),
            centro + new Vector3( ext.x, -ext.y, -ext.z),
            centro + new Vector3(-ext.x,  ext.y,  ext.z),
            centro + new Vector3(-ext.x,  ext.y, -ext.z),
            centro + new Vector3(-ext.x, -ext.y,  ext.z),
            centro + new Vector3(-ext.x, -ext.y, -ext.z)
        };

        float min = Vector3.Dot(pontos[0], direcao);
        float max = min;

        for (int i = 1; i < pontos.Length; i++)
        {
            float valor = Vector3.Dot(pontos[i], direcao);
            min = Mathf.Min(min, valor);
            max = Mathf.Max(max, valor);
        }

        return max - min;
    }

    private void PrepararAutoScroll()
    {
        if (textoRect == null || textoDescricao == null || areaVisivelDoTexto == null)
            return;

        textoDescricao.enableWordWrapping = true;
        textoDescricao.overflowMode = TextOverflowModes.Overflow;

        textoRect.anchorMin = new Vector2(0f, 1f);
        textoRect.anchorMax = new Vector2(1f, 1f);
        textoRect.pivot = new Vector2(0.5f, 1f);

        float alturaAreaVisivel = areaVisivelDoTexto.rect.height;
        float originalSizeDeltaX = textoRect.sizeDelta.x;

        Canvas.ForceUpdateCanvases();
        textoDescricao.ForceMeshUpdate();

        float larguraTexto = textoRect.rect.width;

        if (larguraTexto <= 0f)
            larguraTexto = areaVisivelDoTexto.rect.width;

        float alturaPreferida = textoDescricao.GetPreferredValues(textoDescricao.text, larguraTexto, 0f).y;

        float paddingDinamico = alturaPreferida * paddingRelativo;
        float paddingTopo = Mathf.Max(paddingTopoMinimo, paddingDinamico);
        float paddingFinal = Mathf.Max(paddingFinalMinimo, paddingDinamico);

        float alturaComPadding = alturaPreferida + paddingTopo + paddingFinal;

        textoRect.sizeDelta = new Vector2(originalSizeDeltaX, alturaComPadding);

        posicaoInicialTexto = new Vector2(textoRect.anchoredPosition.x, -paddingTopo);
        textoRect.anchoredPosition = posicaoInicialTexto;

        alturaMaximaScroll = Mathf.Max(0f, alturaComPadding - alturaAreaVisivel);

        painelAberto = true;
        tempoQueAbriu = Time.time;
    }
}