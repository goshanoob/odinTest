using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "EntityConfigs/MaterialSoundBinder")]
public class MaterialSoundBinder : SerializedScriptableObject
{
    [TableList] [NonSerialized, OdinSerialize]  private List<MaterialSoundsGroup> _materialSounds = null;

    [Header("Материал по умолчанию для любого коллайдера")]
    [NonSerialized, OdinSerialize]  private PhysicMaterial _defaultMaterial = null;

    public List<MaterialSoundsGroup> MaterialSounds => _materialSounds;
    public PhysicMaterial DefaultMaterial => _defaultMaterial;

    private void OnValidate()
    {
        var duplicates = _materialSounds.GroupBy(item => new {item.SoundEvent})
            .Where(group => group.Count() > 1);

        duplicates.ForEach(item =>
            Debug.LogError(
                $"В ScriptableObject 'MaterialSoundBinder' повторяется группа событий звуков: {item.Key.SoundEvent}")
        );

        _materialSounds.ForEach(item =>
        {
            if (item.SoundSettings == null)
                Debug.LogError(
                    $"В ScriptableObject 'MaterialSoundBinder' не указан Sound Settings у события {item.SoundEvent}");

            if (item.MaterialSounds == null)
                Debug.LogError($"В ScriptableObject 'MaterialSoundBinder' отсутствуют звуки события {item.SoundEvent}");
            else
                item.MaterialSounds.ForEach(element =>
                {
                    if (element.Material == null)
                        Debug.LogError(
                            $"В ScriptableObject 'MaterialSoundBinder' для события {item.SoundEvent} не указан материал.");

                    if (element.SoundsSet == null)
                        Debug.LogError(
                            $"В ScriptableObject 'MaterialSoundBinder' у события {item.SoundEvent} отстутствует набор звуков.");

                    element.SoundsSet.ForEach((clip, index) =>
                    {
                        if (clip == null)
                            Debug.LogError(
                                $"В ScriptableObject 'MaterialSoundBinder' у события {item.SoundEvent}, материал {element.Material}, найден пустой клип #{index + 1}!");
                    });
                });
        });
    }
}

[Serializable]
public class MaterialSoundsGroup
{
    [VerticalGroup("Событие"), HideLabel, TableColumnWidth(50)] [SerializeField]
    private SoundEvents _soundEvent;

    [VerticalGroup("Источник"), LabelWidth(100), TableColumnWidth(150)] [SerializeField]
    private AudioSource _soundSettings;

    [VerticalGroup("Источник"), LabelWidth(100), TableColumnWidth(150)] [SerializeField]
    private float _minPitch = 0.98f;

    [VerticalGroup("Источник"), LabelWidth(100), TableColumnWidth(150)] [SerializeField]
    private float _maxPitch = 1.03f;

    [VerticalGroup("Источник"), LabelWidth(100), TableColumnWidth(150)] [SerializeField]
    private float _minVolume = 0.98f;

    [VerticalGroup("Источник"), LabelWidth(100), TableColumnWidth(150)] [SerializeField]
    private float _maxVolume = 1.03f;

    [VerticalGroup("Звуки"), LabelWidth(100), TableColumnWidth(400)] [SerializeField]
    private MaterialSounds[] _materialSounds;

    public SoundEvents SoundEvent => _soundEvent;
    public AudioSource SoundSettings => _soundSettings;
    public float MinPitch => _minPitch;
    public float MaxPitch => _maxPitch;
    public float MinVolume => _minVolume;
    public float MaxVolume => _maxVolume;
    public MaterialSounds[] MaterialSounds => _materialSounds;
}

[Serializable]
public class MaterialSounds
{
    [NonSerialized, OdinSerialize] private PhysicMaterial _material;
    [NonSerialized, OdinSerialize] private AudioClip[] _soundsSet;

    public PhysicMaterial Material => _material;
    public AudioClip[] SoundsSet => _soundsSet;
}

public enum SoundEvents {
    SoldierStep = 0,
    SoldierJump = 1,
    SoldierLanded = 2,
    SoldierFell = 3,
    SoldierKilledDropped = 4,
    BulletHit = 5,
    KnifeHit = 6,
    DroneHit = 7,
    GrenadeHit = 8,
}