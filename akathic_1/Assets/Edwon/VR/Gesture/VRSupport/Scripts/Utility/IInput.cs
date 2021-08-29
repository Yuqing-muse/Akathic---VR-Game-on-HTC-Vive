using UnityEngine;
using System.Collections;

namespace Edwon.VR.Input
{
    public static class InputOptions
    {
        public enum Button
        {
            Button1,
            Button2,
            Button3,
            Button4,
            Directional1,
            Trigger1,
            Trigger2,
            Platform
        };
        public enum Touch
        {
            Trigger1,
            Trigger2
        }
        public enum Axis1D
        {
            Trigger1,
            Trigger2
        };
        public enum Axis2D
        {
            Directional1
        };
    }

    public interface IInput
    {
        bool GetButton(InputOptions.Button button);

        bool GetButtonDown(InputOptions.Button button);

        bool GetButtonUp(InputOptions.Button button);

        bool GetTouch(InputOptions.Touch touch);

        bool GetTouchDown(InputOptions.Touch touchDown);

        float GetAxis1D(InputOptions.Axis1D axis1D);

        Vector2 GetAxis2D(InputOptions.Axis2D axis2D);

        void InputUpdate();

    }
}