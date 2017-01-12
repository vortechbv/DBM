Option Explicit
Option Strict

' DBM
' Dynamic Bandwidth Monitor
' Leak detection method implemented in a real-time data historian
'
' Copyright (C) 2014, 2015, 2016, 2017  J.H. Fitié, Vitens N.V.
'
' This file is part of DBM.
'
' DBM is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' DBM is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with DBM.  If not, see <http://www.gnu.org/licenses/>.

Namespace Vitens.DynamicBandwidthMonitor

    Public Class DBMResult

        Public Prediction As DBMPrediction
        Public Factor, OriginalFactor, AbsoluteErrors(), RelativeErrors() As Double
        Public AbsoluteErrorStats, RelativeErrorStats As New DBMStatistics
        Public SuppressedBy As DBMPointDriver

        Public Sub New
            ReDim AbsoluteErrors(DBMParameters.CorrelationPreviousPeriods)
            ReDim RelativeErrors(DBMParameters.CorrelationPreviousPeriods)
        End Sub

        Public Sub Calculate(Index As Integer, MeasuredValueEMA As Double, PredictedValueEMA As Double, LowerControlLimitEMA As Double, UpperControlLimitEMA As Double) ' Calculates and stores prediction errors and initial results
            AbsoluteErrors(Index) = PredictedValueEMA-MeasuredValueEMA ' Absolute prediction error (for prediction error correlation calculations)
            RelativeErrors(Index) = PredictedValueEMA/MeasuredValueEMA-1 ' Relative prediction error (for prediction error correlation calculations)
            If Prediction Is Nothing Then ' Store initial (no time offset because of prediction error correlation calculations) results
                Prediction = New DBMPrediction(MeasuredValueEMA, PredictedValueEMA, LowerControlLimitEMA, UpperControlLimitEMA)
                If MeasuredValueEMA < LowerControlLimitEMA Then ' Lower control limit exceeded
                    Factor = (PredictedValueEMA-MeasuredValueEMA)/(LowerControlLimitEMA-PredictedValueEMA)
                ElseIf MeasuredValueEMA > UpperControlLimitEMA Then ' Upper control limit exceeded
                    Factor = (MeasuredValueEMA-PredictedValueEMA)/(UpperControlLimitEMA-PredictedValueEMA)
                End If
                OriginalFactor = Factor ' Store original factor before possible suppression
            End If
        End Sub

    End Class

End Namespace
