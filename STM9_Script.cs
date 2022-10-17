using Receiver2ModdingKit;
using Unity;
using UnityEngine;
using Receiver2;
using RewiredConsts;
using System.Linq;

namespace STM9_plugin
{
    public class STM9Script : ModGunScript
    {
        //3/4 of the code already comes from the UMP, cheers!
        private float hammer_accel = -5000;
        private float m_charging_handle_amount;
        private ModHelpEntry help_entry;
        public Sprite help_entry_sprite;
        private readonly float[] slide_push_hammer_curve = new float[] {
            0,
            0,
            0.2f,
            1
        };
        public override ModHelpEntry GetGunHelpEntry()
        {
            return help_entry = new ModHelpEntry("STM-9")
            {
                info_sprite = help_entry_sprite,
                title = "Soyuz-TM STM-9",
                description = "Soyuz-TM STM-9 Gen.2 9x19m carbine\n"
                            + "Capacity: 33 + 1, 9x19mm NATO\n"
                            + "\n"
                            + "Made with the help of world Semi-Auto Rifle bronze medalist Vadim Mikhailov, the STM-9 is part of a series of reliable sporting rifles, manufactured by the Saint-Petersburg based company, Soyuz-TM. \n"
                            + "\n"
                            + "Being one of the leading manufacturers of the commercial sports market in Russia, this carbine features excellent out-of-the-box performance, while still mainting a lot of customization options."
            };
        }
        public override LocaleTactics GetGunTactics()
        {
            return new LocaleTactics()
            {
                gun_internal_name = InternalName,
                title = "Soyuz-TM STM-9\n",
                text = "A modded carbine, made with love- XO\n" +
                       "A 9mm carbine made by a leader of the Russian civilian sporting market, this gun features low recoil and high accuracy, while still not being all too cumbersome.\n" +
                       "To safely holster the STM-9, simply flip the safety on."
            };
        }
        public override void InitializeGun()
        {
            pooled_muzzle_flash = ((GunScript)ReceiverCoreScript.Instance().generic_prefabs.First(it => { return it is GunScript && ((GunScript)it).gun_model == GunModel.Glock; })).pooled_muzzle_flash;
            //loaded_cartridge_prefab = ((GunScript)ReceiverCoreScript.Instance().generic_prefabs.First(it => { return it is GunScript && ((GunScript)it).gun_model == GunModel.Glock; })).loaded_cartridge_prefab;
        }
        public override void AwakeGun()
        {
            hammer.amount = 1;
        }
        public override void UpdateGun()
        {
            hammer.asleep = true;
            hammer.accel = hammer_accel;

            if (slide.amount > 0 && _hammer_state != 3)
            { // Bolt cocks the hammer when moving back 
                hammer.amount = Mathf.Max(hammer.amount, InterpCurve(slide_push_hammer_curve, slide.amount));
            }

            if (hammer.amount == 1) _hammer_state = 3;

            if (IsSafetyOn())
            {
                trigger.amount = Mathf.Min(trigger.amount, 0.1f);

                trigger.UpdateDisplay();
            }

            if (hammer.amount == 0 && _hammer_state == 2)
            { // If hammer dropped and hammer was cocked then fire gun and decock hammer
                TryFireBullet(1, FireBullet);

                _hammer_state = 0;

                _disconnector_needs_reset = true;
            }

            if (trigger.amount == 0)
            {
                _disconnector_needs_reset = false;
            }

            if (slide_stop.amount == 1)
            {
                slide_stop.asleep = true;
            }

            if (slide.amount == 0 && _hammer_state == 3 && _disconnector_needs_reset == false)
            { // Simulate auto sear
                hammer.amount = Mathf.MoveTowards(hammer.amount, _hammer_cocked_val, Time.deltaTime * Time.timeScale * 50);
                if (hammer.amount == _hammer_cocked_val) _hammer_state = 2;
            }

            if (_hammer_state != 3 && ((trigger.amount == 1 && !_disconnector_needs_reset && slide.amount == 0) || hammer.amount != _hammer_cocked_val))
            {
                hammer.asleep = false;
            }

            hammer.TimeStep(Time.deltaTime);

            if (player_input.GetButton(Action.Pull_Back_Slide) || player_input.GetButtonUp(Action.Pull_Back_Slide))
            {
                m_charging_handle_amount = slide.amount;
            }
            else
            {
                m_charging_handle_amount = Mathf.Min(m_charging_handle_amount, slide.amount);
            }

            ApplyTransform("charging_handle", m_charging_handle_amount, transform.Find("charging_handle"));

            hammer.UpdateDisplay();

            UpdateAnimatedComponents(); 
        }
    }
}
