/*****************************************************************************\
*                                                                             *
* Canvas.js -    Canvas functions, types, and definitions.                    *
*                                                                             *
*               Version 1.00 ★★★★★                                          *
*                                                                             *
*               Copyright (c) 2016-2016, Server. All rights reserved.         *
*               Created by Lord.                                              *
*                                                                             *
*******************************************************************************/

var m_canvas = document.getElementById("mycanvas"); //绘图区域
var m_context = m_canvas.getContext("2d"); //绘图上下文

//-------------------------OW图形框架 HTML5版---------------------------

//坐标
function POINT(x, y) {
    this.x = x; //横坐标
    this.y = y; //纵坐标
};

//大小
function SIZE(cx, cy) {
    this.cx = cx; //长
    this.cy = cy; //宽
};

//矩形
function RECT(left, top, right, bottom) {
    this.left = left; //左侧
    this.top = top; //上侧
    this.right = right; //右侧
    this.bottom = bottom; //底部
};

//绘图类
function CPaint() {
    this.m_moveTo = false;
    this.m_offsetX = 0;
    this.m_offsetY = 0;
    //添加线
    this.AddLine = function (x1, y1, x2, y2) {
        if (!m_moveTo)
        {
            m_moveTo = true;
            m_context.moveTo(x1 + m_offsetX, y1 + m_offsetY);
        }
        m_context.lineTo(x2 + m_offsetX, y2 + m_offsetY);
    };
    //开始路径
    this.BeginPath = function () {
        m_context.beginPath();
        m_isPath = true;
    };
    //开始绘图
    this.BeginPaint = function (rect) {
        m_moveTo = false;
        m_offsetX = 0;
        m_offsetY = 0;
    };
    //闭合路径
    this.CloseFigure = function () {
        m_context.closePath();
    };
    //关闭路径
    this.ClosePath = function () {
        m_moveTo = false;
    };
    //绘制渐变矩形
    this.DrawGradientRect = function (color1, color2, left, top, right, bottom) {
        w = right - left;
        h = bottom - top;
        var startLiner = m_context.createLinearGradient(left + m_offsetX, top + m_offsetY, w, h);
        startLiner.addColorStop(0, color1);
        startLiner.addColorStop(1, color2);
        m_context.fillStyle = startLiner;
        m_context.fillRect(left + m_offsetX, top + m_offsetY, w, h);
    };
    //绘制线
    this.DrawLine = function (color, width, style, x1, y1, x2, y2) {
        m_context.strokeStyle = color;
        m_context.moveTo(x1 + m_offsetX, y1 + m_offsetY);
        m_context.lineTo(x2 + m_offsetX, y2 + m_offsetY);
        m_context.stroke();
    };
    //绘制路径
    this.DrawPath = function (color, width, style) {
        m_context.strokeStyle = color;
        m_context.stroke();
    };
    //绘制矩形
    this.DrawRect = function (color, width, style, left, top, right, bottom) {
        w = right - left;
        h = bottom - top;
        m_context.strokeStyle = color;
        m_context.strokeRect(left + m_offsetX, top + m_offsetY, w, h);
    };
    //绘制文字
    this.DrawText = function (text, color, font, x, y) {
        if (text.length != 0) {
            m_context.font = font;
            m_context.fillStyle = color;
            m_context.fillText(text, x + m_offsetX, y + m_offsetY);
        }
    };
    //结束绘图
    this.EndPaint = function () {

    };
    //填充路径
    this.FillPath = function (color) {
        m_context.fillStyle = color;
        m_context.stroke();
    };
    //绘制矩形
    this.FillRect = function (color, left, top, right, bottom) {
        w = right - left;
        h = bottom - top;
        m_context.fillStyle = color;
        m_context.fillRect(left + m_offsetX, top + m_offsetY, w, h);
    };
    //裁剪
    this.SetClip = function (left, top, right, bottom) {
        w = right - left;
        h = bottom - top;
        m_context.restore();
        m_context.rect(left + m_offsetX, top + m_offsetY, w, h);
        ctx.stroke();
        m_context.clip();
        m_context.save();
    };
    //设置偏移量
    this.SetOffSet = function (offsetX, offsetY) {
        m_offsetX = offsetX;
        m_offsetY = offsetY;
    };
    //获取文字出来
    this.TextSize = function (text, font) {
        m_context.font = font;
        var tSize = new SIZE(m_context.measureText(text).width, m_context.font.size);
        return tSize;
    };
};

//基础控件
function ControlA() {
    this.m_backColor = null; //背景色
    this.m_borderColor = null; //变现色
    this.m_controls = null; //子控件
    this.m_foreColor = null; //前景色
    this.m_location = null; //坐标
    this.m_name = null; //名称
    this.m_parent = null; //父控件
    this.m_size = null; //大小
    this.m_text = null; //文字
    this.m_visible = false; //可见性
};

var m_controls = new Array; //控件集合
var m_mouseDownControl = null;
var m_mouseDownPoint = new POINT();
var m_mouseMoveControl = null;
var m_paint = new CPaint(); //绘图对象

//Ajax请求数据
var Ajax = function (type, url, dataType, successCallBack) {
    $.ajax({
        type: type,
        url: url,
        dataType: dataType,
        jsonp: 'callback',
        jsonpCallback: "successCallback",
        success: successCallBack
    });
};

//获取绝对位置X
var ClientX = function (control) {
    if (control)
    {
        cLeft = control.m_location.x;
        if (control.m_parent)
        {
            return cLeft + ClientX(control.m_parent);
        }
        else
        {
            return cLeft;
        }
    }
    else
    {
        return 0;
    }
};

//获取绝对位置Y
var ClientY = function (control) {
    if (control)
    {
        cTop = control.m_location.y;
        if (control.m_parent)
        {
            return cTop + ClientY(control.m_parent);
        }
        else
        {
            return cTop;
        }
    }
    else
    {
        return 0;
    }
};

//是否包含坐标
var ContainsPoint = function (control, mp) {
    var clientX = ClientX(control);
    var clientY = ClientY(control);
    var location = control.m_location;
    var size = control.m_size;
    var cp = new POINT(mp.x - clientX, mp.y - clientY);
    if (cp.x >= 0 && cp.x <= size.cx
        && cp.y >= 0 && cp.y <= size.cy) {
        return true;
    }
    else {
        return false;
    }
};

//根据坐标查找控件
var FindControl = function(mp, controls)
{
    size = controls.length;
    for (i = size - 1; i >= 0; i--)
    {
        control = controls[i];
        if (control.m_visible)
        {
            if (ContainsPoint(control, mp))
            {
                if(control.m_controls)
                {
                    subControl = FindControl(mp, subControls);
                    if (subControl)
                    {
                        return subControl;
                    }
                }
                return control;
            }
        }
    }
    return null;
};

//获取点击位置
var GetMousePostion = function (position) {
    var x, y;
    if (position.layerX || position.layerX == 0) {
        x = position.layerX;
        y = position.layerY;
    } else if (position.offsetX || position.offsetX == 0) {
        x = position.offsetX;
        y = position.offsetY;
    }
    var bbox = m_canvas.getBoundingClientRect();
    rreturn new POINT((x - bbox.left) * (m_canvas.width / bbox.width), (bbox.top - y) * (m_canvas.height / bbox.height));
}

//刷新方法
var Invalidate = function () {
    m_canvas.setAttribute("width", window.innerWidth);
    m_canvas.setAttribute("height", window.innerHeight);
    m_paint.BeginPaint(null);
    RenderControls(m_controls, null);
    m_paint.EndPaint();
};

//刷新方法
var InvalidateRect = function (control) {
    m_paint.BeginPaint(null);
    var clientX = ClientX(control);
    var clientY = ClientY(control);
    m_paint.SetOffSet(clientX, clientY);
    var mp = control.m_location;
    var drawRect = new RECT(0, 0, control.m_size.cx, control.m_size.cy);
    OnPaint(control, m_paint, drawRect);
    m_paint.EndPaint();
};

//图层鼠标按下事件
m_canvas.onmousedown = function (position) {
    var mp = GetMousePostion(position);
    m_mouseDownControl = null;
    m_mouseDownPoint = mp;
    var control = FindControl(mp, m_controls);
    if(control)
    {
        var cmp = new POINT(mp.x - ClientX(control), mp.y - ClientY(control));
        m_mouseDownControl = control;
        OnMouseDown(m_mouseDownControl, cmp, 1, 1, 0); 
    }
};

//图层鼠标移动事件
m_canvas.onmousemove = function (position) {
    var mp = GetMousePostion(position);
    if(m_mouseDownControl)
    {
        var cmp = new POINT(mp.x - ClientX(m_mouseDownControl), mp.y - ClientY(m_mouseDownControl));
        OnMouseMove(m_mouseDownControl, cmp, 1, 1, 0);
    }
};

////图层鼠标抬起事件
m_canvas.onmouseup = function (position) {
    var mp = GetMousePostion(position);
    if(m_mouseDownControl)
    {
        var cmp = new POINT(mp.x - ClientX(m_mouseDownControl), mp.y - ClientY(m_mouseDownControl));
        var control = FindControl(mp, m_controls);
        if(control != null && control == m_mouseDownControl)
        {
            OnClick(m_mouseDownControl, cmp, 1, 1, 0);
        }
        else
        {
            m_mouseMoveControl = null;
        }
        if(m_mouseDownControl)
        {
            var mouseDownControl = m_mouseDownControl;
            m_mouseDownControl = null;
            OnMouseUp(mouseDownControl, cmp, 1, 1, 0);
        }
    }
};

//重置大小
window.onresize = function () {
    Invalidate();
}

//重绘控件
var RenderControls = function (controls, rect) {
    controlsSize = m_controls.length;
    for (i = 0; i < controlsSize; i++) {
        var control = m_controls[i];
        if (control.m_visible)
        {
            var clientX = ClientX(control);
            var clientY = ClientY(control);
            m_paint.SetOffSet(clientX, clientY);
            var mp = control.m_location;
            var drawRect = new RECT(0, 0, control.m_size.cx, control.m_size.cy);
            OnPaint(control, m_paint, drawRect);
            if (control.m_controls) {
                subControlsSize = control.m_controls.length;
                if (subControlsSize > 0) {
                    RenderControls(control.m_controls, rect);
                }
            }
        }
    }
};

var m_right = new ControlA();
m_right.m_name = "正确";
m_right.m_visible = true;
m_right.m_size = new SIZE(100, 100);
m_right.m_location = new POINT(50, 0);
m_controls.push(m_right);

var m_half = new ControlA();
m_half.m_name = "一半";
m_half.m_visible = true;
m_half.m_size = new SIZE(100, 100);
m_half.m_location = new POINT(50, 110);
m_controls.push(m_half);
var m_error = new ControlA();
m_error.m_name = "错误";
m_error.m_visible = true;
m_error.m_size = new SIZE(100, 100);
m_error.m_location = new POINT(50, 220);
m_controls.push(m_error);
var m_clear = new ControlA();
m_clear.m_name = "重置";
m_clear.m_visible = true;
m_clear.m_size = new SIZE(100, 100);
m_clear.m_location = new POINT(50, 330);
m_controls.push(m_clear);

//-------------------------事件对接---------------------------

//控件的鼠标点击方法
var OnClick = function (control, mp, buttons, clicks, delta) {
    if(control.m_name == "正确")
    {
        Ajax("GET", "http:47.100.16.237:10009/answer?result=2", "html", function (data) {
        });
    }
    else if(control.m_name == "一半")
    {
        Ajax("GET", "http:47.100.16.237:10009/answer?result=1", "html", function (data) {
            });
    }
    else if(control.m_name == "错误")
    {
        Ajax("GET", "http:47.100.16.237:10009/answer?result=0", "html", function (data) {
            });
    }
    else if(control.m_name == "重置")
    {
        Ajax("GET", "http:47.100.16.237:10009/clear", "html", function (data) {
            });
    }
};

//控件的鼠标按下方法
var OnMouseDown = function (control, mp, buttons, clicks, delta) {
    if (control == m_chart1 || control == m_chart2) {
        var draw = false;
        var scrollBarY = control.m_size.cy - m_scrollBarHeight;
        if (mp.x > control.m_scrollEndX - 8 && mp.y > scrollBarY + 18 && mp.x < control.m_scrollEndX + 9
            && mp.y < scrollBarY + m_scrollBarHeight - 18) {
            control.m_isScrollEndDown = true;
            draw = true;
        }
        if (mp.x > control.m_scrollStartX - 8 && mp.y > scrollBarY + 18 && mp.x < control.m_scrollStartX + 9
            && mp.y < scrollBarY + m_scrollBarHeight - 18) {
            control.m_isScrollStartDown = true;
            draw = true;

        }
        if ((mp.x > control.m_scrollEndX - 8 && mp.y > scrollBarY + 18 && mp.x < control.m_scrollEndX + 9
            && mp.y < scrollBarY + m_scrollBarHeight - 18)
            && (mp.x > control.m_scrollStartX - 8 && mp.y > scrollBarY + 18
            && mp.x < control.m_scrollStartX + 9 && mp.y < scrollBarY + m_scrollBarHeight - 18)) {
            if (control.m_scrollEndZ > m_scrollStartZ) {
                control.m_isScrollEndDown = true;
                control.m_isScrollStartDown = false;
                draw = true;
            }
            else {
                control.m_isScrollStartDown = true;
                control.m_isScrollEndDown = false;
                draw = true;
            }
        }
        if (draw) {
            Invalidate();
        }
    }
};

//控件的鼠标移动方法
var OnMouseMove = function (control, mp, buttons, clicks, delta) {
    if (control == m_chart1 || control == m_chart2) {
        var draw = false;
        if (control.m_isScrollEndDown) {
            control.m_scrollEndIndex = GetIndex(control, mp.x);
            draw = true;
        }
        if (control.m_isScrollStartDown) {
            control.m_scrollStartIndex = GetIndex(control, mp.x);
            draw = true;
        }
        if (draw) {
            InvalidateRect(control);
        }
    }
};

//控件的鼠标抬起方法
var OnMouseUp = function (control, mp, buttons, clicks, delta) {
    if (control == m_chart1 || control == m_chart2)
    {
        if (control.m_isScrollEndDown || control.m_isScrollStartDown) {
            control.m_isScrollEndDown = false;
            control.m_isScrollStartDown = false;
            Invalidate();
        }
    }
};
//控件的重绘方法
var OnPaint = function (control, paint, clipRect) {
    if(control.m_name == "正确")
    {
        paint.FillRect("rgb(0,255,0)", 0, 0, control.m_size.cx, control.m_size.cy);
        paint.DrawText("正确", "rgb(0,0,0)", "30px Arial", 0, 30);
    }
    else if(control.m_name == "一半")
    {
        paint.FillRect("rgb(255,255,0)", 0, 0, control.m_size.cx, control.m_size.cy);
        paint.DrawText("一半", "rgb(0,0,0)", "30px Arial", 0, 30);
    }
    else if(control.m_name == "错误")
    {
        paint.FillRect("rgb(255,0,0)", 0, 0, control.m_size.cx, control.m_size.cy);
        paint.DrawText("错误", "rgb(0,0,0)", "30px Arial", 0, 30);
    }
    else if(control.m_name == "重置")
    {
        paint.FillRect("rgb(0,255,255)", 0, 0, control.m_size.cx, control.m_size.cy);
        paint.DrawText("重置", "rgb(0,0,0)", "30px Arial", 0, 30);
    }
    paint.DrawRect("rgb(0,0,0)", 1, 0, 0, 0, control.m_size.cx, control.m_size.cy);
};

Invalidate();
