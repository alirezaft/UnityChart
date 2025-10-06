using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart.Editor
{
    public class Ticks
    {
        private float m_TickLength;
        private float m_Height;
        private float m_Width;
        private int m_DataLength;

        private Axis m_Axis;
        private Painter2D m_Painter;
        private ChartLayout m_ChartLayout;

        public Ticks(Axis axis, int dataLength, float tickLength, ChartLayout layout)
        {
            m_Axis = axis;
            m_DataLength = dataLength;
            m_TickLength = tickLength;
            m_ChartLayout = layout;
        }

        public void PlaceTicksOnYAxis(int ticksCount)
        {
            var tickDistance = m_ChartLayout.ChartHeight / (ticksCount - 1);
            var tickVector = new Vector2(m_TickLength, 0);
            var painterStepVector = new Vector2(0, tickDistance);

            m_Painter.strokeColor = Color.white;
            var tickX = m_ChartLayout.WidthOffset + m_ChartLayout.XStart;
            
            for (float i = m_ChartLayout.YStart; i <= m_ChartLayout.ChartHeight + m_ChartLayout.YStart; i += tickDistance)
            {
                m_Painter.BeginPath();

                var currPos = new Vector2(tickX, i);
                m_Painter.MoveTo(currPos);
                m_Painter.LineTo(currPos + tickVector);
                
                m_Painter.Stroke();
                m_Painter.ClosePath();
            }
        }
        
        public void PlaceTicksOnXAxis(List<float> ticksList, Painter2D painter)
        {
            var numberOfTicks = ticksList.Count - 1;

            var dataSteps = m_ChartLayout.AllowedWidth / (m_DataLength - 1);
            var dataToTickRatio = ((float)m_DataLength - 1) / (ticksList.Count - 1);


            var tickDistance = dataSteps * dataToTickRatio;
            var painterMovementVector = new Vector2(tickDistance, -m_TickLength);
            var tickLengthVector = new Vector2(0, m_TickLength);
            var currPosition = new Vector2(0, m_Axis.ZeroOnYAxis) + tickLengthVector;

            painter.MoveTo(new Vector2(0, m_Axis.ZeroOnYAxis) + tickLengthVector);
            painter.strokeColor = Color.white;

            for (int i = 0; i < numberOfTicks; i++)
            {
                painter.BeginPath();
                painter.MoveTo(currPosition + painterMovementVector);
                currPosition += painterMovementVector;
                painter.LineTo(currPosition + (tickLengthVector / 2));
                painter.LineTo(currPosition - tickLengthVector / 2);
                currPosition += tickLengthVector;
                painter.Stroke();
                painter.ClosePath();
            }
        }

        public void SetPainter(Painter2D painter)
        {
            m_Painter = painter;
        }

        public void SetDimensions(float height, float width)
        {
            m_Height = height;
            m_Width = width;
        }
    }
}