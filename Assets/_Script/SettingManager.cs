using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{

    [Header("Panels")]
    public GameObject settingsPanel;         // Panel cài đặt (SettingsPanel)
    public GameObject confirmationDialog;    // Cửa sổ xác nhận (ConfirmationDialog)

    [Header("Buttons")]
    public Button openSettingsButton;   // Nút Setting trên Menu
    public Button closeSettingsButton;  // Nút đóng SettingsPanel
    public Button resetButton;          // Nút Reset Data trong SettingsPanel
    public Button confirmResetButton;   // Nút xác nhận Reset trong ConfirmationDialog
    public Button cancelResetButton;    // Nút Cancel trong ConfirmationDialog
    [Header("Volume Sliders")]
    public Slider bgmVolumeSlider;      // Slider điều chỉnh âm lượng nhạc nền
    public Slider sfxVolumeSlider;      // Slider điều chỉnh âm lượng hiệu ứng âm thanh
    

    void Start()
    {
        // Ẩn panel cài đặt và cửa sổ xác nhận khi bắt đầu
        settingsPanel.SetActive(false);
        confirmationDialog.SetActive(false);

        // Gán sự kiện cho các nút
        openSettingsButton.onClick.AddListener(OpenSettings);
        closeSettingsButton.onClick.AddListener(CloseSettings);
        resetButton.onClick.AddListener(OnResetButtonClicked);
        confirmResetButton.onClick.AddListener(ConfirmReset);
        cancelResetButton.onClick.AddListener(CancelReset);
        // Khởi tạo giá trị của slider dựa trên AudioManager (nếu AudioManager đã được khởi tạo)
        if (AudioManager.Instance != null)
        {
            bgmVolumeSlider.value = AudioManager.Instance.bgmVolume;
            sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
        }

        // Gán sự kiện khi giá trị slider thay đổi
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OnResetButtonClicked()
    {
        // Hiển thị cửa sổ xác nhận
        confirmationDialog.SetActive(true);
    }

    public void ConfirmReset()
    {
        // Thực hiện reset dữ liệu, ví dụ: xóa toàn bộ PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerData.gold = 1000; // Đặt lại giá trị mặc định
        PlayerPrefs.SetInt("PlayerGold", PlayerData.gold);
        PlayerPrefs.Save();
        Debug.Log("Data has been reset!");

        // Ẩn cửa sổ xác nhận và có thể cập nhật lại UI nếu cần
        confirmationDialog.SetActive(false);
    }

    public void CancelReset()
    {
        // Ẩn cửa sổ xác nhận mà không reset dữ liệu
        confirmationDialog.SetActive(false);
    }
    
    void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(value);
            PlayerPrefs.SetFloat("BGMVolume", value);
            PlayerPrefs.Save();
        }
    }

    void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
            PlayerPrefs.SetFloat("SFXVolume", value);
            PlayerPrefs.Save();
        }
    }
}