using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [Tooltip("AudioSource cho nhạc nền")]
    public AudioSource bgmSource; 
    [Tooltip("AudioSource cho hiệu ứng âm thanh")]
    public AudioSource sfxSource;

    [Header("Enemy shoot Spiral Sound Settings")]
    public AudioSource spiralSoundSource;
    public AudioClip spiralShootClip;

    [Header("Menu & Store Music")]
    [Tooltip("Danh sách các bài nhạc cho Menu & Store")]
    public AudioClip[] menuMusicClips; 

    [Header("Gameplay Music")]
    [Tooltip("Danh sách các bài nhạc cho Gameplay")]
    public AudioClip[] gameplayMusicClips; 

    [Header("SFX Clips")]
    public AudioClip playerHitClip;
    public AudioClip playerDieClip;
    public AudioClip enemyHitClip;
    public AudioClip enemyDieClip;
    public AudioClip playerShootClip;
    public AudioClip enemyShootClip;
    [Tooltip("Các clip cho power up, thứ tự tùy theo loại")]
    public AudioClip[] powerUpClips; 
    public AudioClip storeBuyClip;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float bgmVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    void Awake() {
        // Thiết lập singleton và giữ cho đối tượng không bị phá hủy khi chuyển scene
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", bgmVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// Phát nhạc nền với clip cho trước (sẽ loop)
    /// </summary>
    public void PlayBackgroundMusic(AudioClip clip) {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return; // Không thay đổi nếu đang phát cùng clip
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// Dừng nhạc nền
    /// </summary>
    public void StopBackgroundMusic() {
        bgmSource.Stop();
    }

    /// <summary>
    /// Phát hiệu ứng âm thanh một lần
    /// </summary>
    public void PlaySFX(AudioClip clip) {
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    /// <summary>
    /// Chọn ngẫu nhiên một bài nhạc cho Menu & Store và phát
    /// </summary>
    public void PlayRandomMenuMusic() {
        if (menuMusicClips.Length > 0) {
            int index = Random.Range(0, menuMusicClips.Length);
            PlayBackgroundMusic(menuMusicClips[index]);
        }
    }

    /// <summary>
    /// Chọn ngẫu nhiên một bài nhạc cho Gameplay và phát
    /// </summary>
    public void PlayRandomGameplayMusic() {
        if (gameplayMusicClips.Length > 0) {
            int index = Random.Range(0, gameplayMusicClips.Length);
            PlayBackgroundMusic(gameplayMusicClips[index]);
        }
    }

    /// <summary>
    /// Cập nhật volume nhạc nền theo giá trị được đặt trong Settings
    /// </summary>
    public void SetBGMVolume(float volume) {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    /// <summary>
    /// Cập nhật volume hiệu ứng âm thanh theo giá trị được đặt trong Settings
    /// </summary>
    public void SetSFXVolume(float volume) {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// Cho phép bật/tắt hiệu ứng âm thanh
    /// </summary>
    public void ToggleSFX(bool enabled) {
        sfxSource.mute = !enabled;
    }
}
