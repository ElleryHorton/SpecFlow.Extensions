using OpenQA.Selenium;
using SpecFlow.Extensions.Web.ByWrappers;
using SpecFlow.Extensions.WebDriver.PortalDriver;
using System;
using System.Globalization;

namespace PageObjects
{
    public class TestCalendarPage
    {
        public ByEx ThemeDropDown { get { return new ByEx(By.Id("combo-1038-inputEl")); } }
        //public ByText MiniCalendarMonthDropDown { get { return new ByText(By.TagName("span"), string.Format("{0} {1}", DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture), DateTime.Now.Year)); } }
        public ByEx MiniCalendarMonthDropDown { get { return new ByEx(By.Id("splitbutton-1035")); } }
        public ByText MiniCalendarOkButton { get { return new ByText(By.TagName("span"), "OK"); } }
        public void MiniCalendarSelectDate(IPortalDriver d, string month, string year)
        {
            d.Click(new ByEx(By.LinkText(month)));
            d.Click(new ByEx(By.LinkText(year)));
            d.Click(MiniCalendarOkButton);
        }

        public ByEx CalendarDay { get { return new ByEx(By.ClassName("ext-cal-day")); } }
        public ByEx CalendarDate(DateTime date)
        {
            return new ByEx(By.Id(string.Format("app-calendar-month-day-{0}", date.ToString("yyyyMMdd"))));
        }
        public ByEx EditDetails { get { return new ByEx(By.LinkText("Edit Details...")); } }
        public ByAttribute AddEventTitle { get { return new ByAttribute(By.TagName("input"), "name", "Title"); } }
        public ByText AddEventSaveButton { get { return new ByText(By.TagName("span"), "Save"); } }
        public ByText AddEventCancelButton { get { return new ByText(By.TagName("span"), "Cancel"); } }
    }
}
