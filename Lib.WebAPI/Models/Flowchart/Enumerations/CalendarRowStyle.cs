namespace Lib.WebAPI.Models.Flowchart.Enumerations
{
    /// <summary>
    /// CalendarRowStyle
    /// </summary>
    public enum CalendarRowStyle
    {
        // Month Row Styles ------------------------------------------------------

        /// <summary>
        /// The month MMM
        /// </summary>
        MonthMMM = 0,

        /// <summary>
        /// The month mm
        /// </summary>
        MonthMM = 1,

        // PeriodMonth Row Styles ------------------------------------------------

        /// <summary>
        /// The period month p1
        /// </summary>
        PeriodMonthP1 = 100,

        // PeriodWeek Row Styles -------------------------------------------------

        /// <summary>
        /// The period week one x one
        /// </summary>
        PeriodWeekOneXOne = 200,

        // Quarter Row Styles ----------------------------------------------------

        /// <summary>
        /// The quarter q1
        /// </summary>
        QuarterQ1 = 300,

        // Week Row Styles -------------------------------------------------------

        /// <summary>
        /// The week dd
        /// </summary>
        WeekDD = 400,

        // Year Row Styles -------------------------------------------------------

        /// <summary>
        /// The year yyyy
        /// </summary>
        YearYYYY = 500,

        /// <summary>
        /// The year yy
        /// </summary>
        YearYY = 501,
    }
}