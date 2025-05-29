using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Добавляем для работы с Image

public class RocketDramaController : MonoBehaviour
{
    [Header("Player Control")]
    [SerializeField] private Moving playerMovement;
    [SerializeField] private Drag playerInteraction;

    [Header("Dialog System")]
    [SerializeField] private NewCommentSystem commentController;
    [SerializeField] private List<string> dialogSequence = new List<string>();
    [SerializeField] private float timeBetweenDialogs = 2f;

    [Header("Effects")]
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip sirenClip;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeDuration = 1.5f;
    [SerializeField] private float shakeIntensity = 0.7f;

    [Header("Avatar Settings")]
    [SerializeField] private Sprite avatarSprite; // Спрайт аватара
    [SerializeField] private Image avatarImageUI; // UI Image для отображения
    [SerializeField] private float fadeDuration = 1f; // Длительность появления/исчезания
    [SerializeField] private List<string> avatarDialog;

    private bool isSequenceActive = false;
    private AudioSource audioSource;
    private CanvasGroup avatarCanvasGroup; // Для плавного появления

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Настраиваем UI Image
        if (avatarImageUI != null)
        {
            avatarCanvasGroup = avatarImageUI.GetComponent<CanvasGroup>();
            if (avatarCanvasGroup == null)
            {
                avatarCanvasGroup = avatarImageUI.gameObject.AddComponent<CanvasGroup>();
            }
            avatarImageUI.sprite = avatarSprite;
            avatarImageUI.preserveAspect = true;
            HideAvatarImmediately();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSequenceActive)
        {
            StartCoroutine(DramaSequence());
        }
    }

    private IEnumerator DramaSequence()
    {
        isSequenceActive = true;
        DisablePlayerControl();

        // Показываем диалоги
        foreach (var dialog in dialogSequence)
        {
            commentController.ShowComment(dialog);
            yield return new WaitForSeconds(timeBetweenDialogs);
        }

        // Эффекты
        if (sirenClip != null) audioSource.PlayOneShot(sirenClip);
        if (explosionClip != null) audioSource.PlayOneShot(explosionClip);
        if (cameraShake != null) cameraShake.ShakeCamera(shakeDuration, shakeIntensity);

        yield return new WaitForSeconds(shakeDuration);

        // Показываем аватар
        if (avatarImageUI != null && avatarSprite != null)
        {
            yield return StartCoroutine(ShowAvatar());

            // Диалоги аватара
            foreach (var avatarLine in avatarDialog)
            {
                commentController.ShowComment(avatarLine);
                yield return new WaitForSeconds(timeBetweenDialogs);
            }

            yield return StartCoroutine(HideAvatar());
        }

        EnablePlayerControl();
        isSequenceActive = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    private IEnumerator ShowAvatar()
    {
        avatarImageUI.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            avatarCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        avatarCanvasGroup.alpha = 1f;
    }

    private IEnumerator HideAvatar()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            avatarCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        HideAvatarImmediately();
    }

    private void HideAvatarImmediately()
    {
        if (avatarCanvasGroup != null)
        {
            avatarCanvasGroup.alpha = 0f;
            avatarImageUI.gameObject.SetActive(false);
        }
    }

    private void DisablePlayerControl()
    {
        if (playerMovement != null) playerMovement.DisableAllInput();
        if (playerInteraction != null) playerInteraction.enabled = false;
    }

    private void EnablePlayerControl()
    {
        if (playerMovement != null) playerMovement.EnableAllInput();
        if (playerInteraction != null) playerInteraction.enabled = true;
    }
}