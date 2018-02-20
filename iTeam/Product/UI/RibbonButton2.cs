using System;
using System.Collections.Generic;
using System.Text;

namespace OwLib
{
    /// <summary>
    /// ͸����ť
    /// </summary>
    public class RibbonButton2 : ButtonA
    {
        /// <summary>
        /// ������ť
        /// </summary>
        public RibbonButton2()
        {
            BorderColor = COLOR.EMPTY;
            ForeColor = CDraw.PCOLORS_FORECOLOR4;
        }

        /// <summary>
        /// ��ȡ���ڻ��Ƶı���ɫ
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
        /// �ػ汳������
        /// </summary>
        /// <param name="paint">��ͼ����</param>
        /// <param name="clipRect">�ü�����</param>
        public override void OnPaintBackground(CPaint paint, RECT clipRect)
        {
            int width = Width - 1, height = Height - 1;
            RECT drawRect = new RECT(0, 0, width, height);
            paint.FillRoundRect(GetPaintingBackColor(), drawRect, 4);
        }
    }
}
