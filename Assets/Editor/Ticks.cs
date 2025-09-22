using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChart
{
    public class Ticks
    {
        private float m_TickLength;
        private float m_Height;
        private float m_Width;
        private int m_Length;

        private Axis m_Axis;
        private Painter2D m_Painter;

        public Ticks(Axis axis, int length, float tickLength)
        {
            m_Axis = axis;
            m_Length = length;
            m_TickLength = tickLength;
        }
        
        public void PlaceTicksOnYAxis(int ticksCount)
        {
            var tickDistance = m_Height / (ticksCount - 1);
            var tickVector = new Vector2(m_TickLength, 0);
            var painterStepVector = new Vector2(-m_TickLength, -tickDistance);

            var currPos = new Vector2(0 + m_TickLength, m_Height);
            m_Painter.MoveTo(currPos);

            for (int i = 0; i < ticksCount; i++)
            {
                m_Painter.BeginPath();
                m_Painter.strokeColor = Color.white;
                m_Painter.MoveTo(currPos + painterStepVector);
                currPos += painterStepVector;
                m_Painter.LineTo(currPos + tickVector * 1.5f);
                currPos += tickVector;
                m_Painter.Stroke();
                m_Painter.ClosePath();
            }
        }
        
        public void PlaceTicksOnXAxis(List<float> ticksList, Painter2D painter)
        {
            var numberOfTicks = ticksList.Count - 1;

            var dataSteps = m_Width / (m_Length - 1);
            var dataToTickRatio = ((float)m_Length - 1) / (ticksList.Count - 1);


            Debug.Log("Num of Ticks: " + numberOfTicks);
            var tickDistance = dataSteps * dataToTickRatio;
            Debug.Log($"Tick distance: {tickDistance}");
            var painterMovementVector = new Vector2(tickDistance, -m_TickLength);
            var tickLengthVector = new Vector2(0, m_TickLength);
            var currPosition = new Vector2(0, m_Axis.ZeroOnYAxis) + tickLengthVector;

            painter.MoveTo(new Vector2(0, m_Axis.ZeroOnYAxis) + tickLengthVector);
            painter.strokeColor = Color.white;

            for (int i = 0; i < numberOfTicks; i++)
            {
                Debug.Log(currPosition);
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