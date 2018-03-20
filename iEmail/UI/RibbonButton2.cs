using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// 透明按钮
    /// </summary>
    public class RibbonButton2 : ButtonA
    {
        /// <summary>
        /// 创建按钮
        /// </summary>
        public RibbonButton2()
        {
            BorderColor = COLOR.EMPTY;
            ForeColor = CDraw.PCOLORS_FORECOLOR4;
        }

        /// <summary>
        /// 获取正在绘制的背景色
        /// </summary>
        /// <returns></returns>
        protected override long GetPaintingBackColor()
        {
            if (Native.PushedControl == this)
            {
                return COLOR.Reverse(null, CDraw.PCOLORS_BACKCOLOR8);
            }
            else if (Native.HoveredControl == this)
            {
                return COLOR.RatioColor(null, CDraw.PCOLORS_BACKCOLOR8, 0.95);
            }
            else
            {
                return CDraw.PCOLORS_BACKCOLOR8;
            }
        }

        /// <summary>
        /// 重绘背景方法
        /// </summary>
        /// <param name="paint">绘图对象</param>
        /// <param name="clipRect">裁剪区域</param>
        public override void OnPaintBackground(CPaint paint, RECT clipRect)
        {
            int width = Width - 1, height = Height - 1;
            RECT drawRect = new RECT(0, 0, width, height);
            paint.FillRoundRect(GetPaintingBackColor(), drawRect, 4);
        }
    }
}
