using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViveControllerInputTest : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device device
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void FixedUpdate()
    {   //获取手柄输入  
        

        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("按了 “trigger” “扳机键”");

            //右手震动  
            //拉弓类似操作应该就是按住trigger（扳机）gettouch时持续调用震动方法模拟弓弦绷紧的感觉。  
            var deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
            SteamVR_Controller.Input(deviceIndex2).TriggerHapticPulse(500);//手柄震动函数

        }
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("按下了 “trigger” “扳机键”");

        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("松开了 “trigger” “扳机键”");

            //左手震动  
            var deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
            SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(3000);

            //右手震动  
            var deviceIndex1 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
            SteamVR_Controller.Input(deviceIndex1).TriggerHapticPulse(3000);
        }


        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("用press按下了 “trigger” “扳机键”");
        }
        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("用press按了 “trigger” “扳机键”");
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("用press松开了 “trigger” “扳机键”");
        }

        //system键 圆盘下面那个键   
        // reserved 为Steam系统保留,用来调出Steam系统菜单 因此貌似自己加的功能没啥用  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.System))
        {
            Debug.Log("按下了 “system” “系统按钮/Steam”");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.System))
        {
            Debug.Log("用press按下了 “System” “系统按钮/Steam”");
        }

        //ApplicationMenu键 带菜单标志的那个按键（在方向圆盘上面）  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Debug.Log("按下了 “ApplicationMenu” “菜单键”");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Debug.Log("用press按下了 “ApplicationMenu” “菜单键”");
        }

        //Grip键 两侧的键 每个手柄左右各一功能相同，同一手柄两个键是一个键。  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("按下了 “Grip” “ ”");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("用press按下了 “Grip” “ ”");
        }



        //Axis0键 与圆盘有交互 与圆盘有关  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis0))
        {
            Debug.Log("按下了 “Axis0” “方向 ”");
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis0))
        {
            Debug.Log("用press按下了 “Axis0” “方向 ”");
        }

        //Axis1键  目前未发现按键位置  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis1))
        {
            Debug.Log("按下了 “Axis1” “ ”");
        }
        //按动触发   
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis1))
        {
            Debug.Log("用press按下了 “Axis1” “ ”");
        }

        //Axis2键 目前未发现按键位置  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis2))
        {
            Debug.Log("按下了 “Axis2” “ ”");
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis2))
        {
            Debug.Log("用press按下了 “Axis2” “ ”");
        }

        //Axis3键  未目前未发现按键位置  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis3))
        {
            Debug.Log("按下了 “Axis3” “ ”");
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis3))
        {
            Debug.Log("用press按下了 “Axis3” “ ”");
        }

        //Axis4键  目前未发现按键位置  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis4))
        {
            Debug.Log("按下了 “Axis4” “ ”");
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis4))
        {
            Debug.Log("用press按下了 “Axis4” “ ”");
        }


        ///方向圆盘：  
        //区分了press检测与touch检测的不同之处，圆盘可以触摸，顾名思义，touch检测的是触摸，press检测的是按动
        //Axis0键 与圆盘有交互 与圆盘有关  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis0))
        {
            Debug.Log("按下了 “Axis0” “方向 ”");
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis0))
        {
            Debug.Log("用press按下了 “Axis0” “方向 ”");
        }

        //Axis1键  目前未发现按键位置  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis1))
        {
            Debug.Log("按下了 “Axis1” “ ”");
        }
        //按动触发   
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis1))
        {
            Debug.Log("用press按下了 “Axis1” “ ”");
        }

        //Axis2键 目前未发现按键位置  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis2))
        {
            Debug.Log("按下了 “Axis2” “ ”");
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis2))
        {
            Debug.Log("用press按下了 “Axis2” “ ”");
        }

        //Axis3键  未目前未发现按键位置  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis3))
        {
            Debug.Log("按下了 “Axis3” “ ”");
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis3))
        {
            Debug.Log("用press按下了 “Axis3” “ ”");
        }

        //Axis4键  目前未发现按键位置  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Axis4))
        {
            Debug.Log("按下了 “Axis4” “ ”");
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Axis4))
        {
            Debug.Log("用press按下了 “Axis4” “ ”");
        }


        //ATouchpad键 圆盘交互  
        //触摸触发  
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("按下了 “Touchpad” “ ”");

            //方法返回一个坐标 接触圆盘位置  
            Vector2 cc = device.GetAxis();
            Debug.Log(cc);
            // 例子：圆盘分成上下左右  
            //返回一个夹角 180 到 -180 的角度
            float angle = VectorAngle(new Vector2(1, 0), cc);
            Debug.Log(angle);
            //下  
            if (angle > 45 && angle < 135)
            {
                Debug.Log("下");
            }
            //上  
            if (angle < -45 && angle > -135)
            {
                Debug.Log("上");
            }
            //左  
            if ((angle < 180 && angle > 135) || (angle < -135 && angle > -180))
            {
                Debug.Log("左");
            }
            //右  
            if ((angle > 0 && angle < 45) || (angle > -45 && angle < 0))
            {
                Debug.Log("右");
            }
        }
        //按动触发  
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("用press按下了 “Touchpad” “ ”");
        }
    }

    // Update is called once per frame  
    void Update()
    {

    }


    //方向圆盘最好配合这个使用 圆盘的.GetAxis()会检测返回一个二位向量，可用角度划分圆盘按键数量  
    //这个函数输入两个二维向量会返回一个夹角 180 到 -180  
    float VectorAngle(Vector2 from, Vector2 to)
    {
        float angle;
        Vector3 cross = Vector3.Cross(from, to);
        angle = Vector2.Angle(from, to);
        return cross.z > 0 ? -angle : angle;
    }
}
