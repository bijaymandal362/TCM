using Entities;
using Models.Constant.Authorization;
using Models.Constant.ListItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Data
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            //
            #region ListItemCategory
            var listItemCategories = new List<ListItemCategory>
            {
                new ListItemCategory
                {
                    ListItemCategoryId=1,
                    ListItemCategoryName="Gender",
                    ListItemCategorySystemName="Gender",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                new ListItemCategory
                {
                    ListItemCategoryId=2,
                    ListItemCategoryName="ProjectStatus",
                    ListItemCategorySystemName="ProjectStatus",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },
                new ListItemCategory
                {
                    ListItemCategoryId=3,
                    ListItemCategoryName="ProjectMarket",
                    ListItemCategorySystemName="ProjectMarket",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },
                new ListItemCategory
                {
                    ListItemCategoryId=4,
                    ListItemCategoryName="ProjectSource",
                    ListItemCategorySystemName="ProjectSource",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },

                new ListItemCategory
                {
                    ListItemCategoryId=5,
                    ListItemCategoryName="ProjectType",
                    ListItemCategorySystemName="ProjectType",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },

                new ListItemCategory
                {
                    ListItemCategoryId=6,
                    ListItemCategoryName="ProjectTechnology",
                    ListItemCategorySystemName="ProjectTechnology",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },
                  new ListItemCategory
                {
                    ListItemCategoryId=7,
                    ListItemCategoryName="ProjectRole",
                    ListItemCategorySystemName="ProjectRole",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },

                  new ListItemCategory
                {
                    ListItemCategoryId=8,
                    ListItemCategoryName="UserRole",
                    ListItemCategorySystemName="UserRole",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },

                    new ListItemCategory
                {
                    ListItemCategoryId=9,
                    ListItemCategoryName="ProjectModuleType",
                    ListItemCategorySystemName="ProjectModuleType",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },

                        new ListItemCategory
                {
                    ListItemCategoryId=10,
                    ListItemCategoryName="TestCaseType",
                    ListItemCategorySystemName="TestCaseType",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },
                new ListItemCategory
                {
                    ListItemCategoryId=11,
                    ListItemCategoryName="ThemeType",
                    ListItemCategorySystemName="ThemeType",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },
                
                new ListItemCategory
                {
                    ListItemCategoryId=12,
                    ListItemCategoryName="TestPlanType",
                    ListItemCategorySystemName="TestPlanType",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },

                  new ListItemCategory
                {
                    ListItemCategoryId=13,
                    ListItemCategoryName="Environment",
                    ListItemCategorySystemName="Environment",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },
                     new ListItemCategory
                {
                    ListItemCategoryId=14,
                    ListItemCategoryName="TestRunStatus",
                    ListItemCategorySystemName="TestRunStatus",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true
                },

             };
            var existingListItemsCategoryinDb = context.ListItemCategory.Select(x => x.ListItemCategorySystemName).ToList();
            await context.ListItemCategory.AddRangeAsync(listItemCategories.Where(x => !existingListItemsCategoryinDb.Contains(x.ListItemCategorySystemName)));
            await context.SaveChangesAsync();
            #endregion

            #region ListItem
            var listItems = new List<ListItem>
            {
                new ListItem
                {
                    ListItemId=1,
                    ListItemCategoryId=1,
                    ListItemName="Male",
                    ListItemSystemName="Male",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=2,
                    ListItemCategoryId=1,
                    ListItemName="Female",
                    ListItemSystemName="Female",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                new ListItem
                {
                    ListItemId=3,
                    ListItemCategoryId=2,
                    ListItemName="In-Progress",
                    ListItemSystemName="In-Progress",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=4,
                    ListItemCategoryId=2,
                    ListItemName="Closed",
                    ListItemSystemName="Closed",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=5,
                    ListItemCategoryId=2,
                    ListItemName="Hold",
                    ListItemSystemName="Hold",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=6,
                    ListItemCategoryId=3,
                    ListItemName="Nepal",
                    ListItemSystemName="Nepal",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=7,
                    ListItemCategoryId=3,
                    ListItemName="Singapore",
                    ListItemSystemName="Singapore",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=8,
                    ListItemCategoryId=3,
                    ListItemName="Japan",
                    ListItemSystemName="Japan",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=9,
                    ListItemCategoryId=3,
                    ListItemName="UK",
                    ListItemSystemName="UK",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=10,
                    ListItemCategoryId=3,
                    ListItemName="USA",
                    ListItemSystemName="USA",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=11,
                    ListItemCategoryId=4,
                    ListItemName="In-House",
                    ListItemSystemName="In-House",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=12,
                    ListItemCategoryId=4,
                    ListItemName="Client",
                    ListItemSystemName="Client",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=13,
                    ListItemCategoryId=5,
                    ListItemName="Monthly",
                    ListItemSystemName="Monthly",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=14,
                    ListItemCategoryId=5,
                    ListItemName="Fixed",
                    ListItemSystemName="Fixed",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=15,
                    ListItemCategoryId=6,
                    ListItemName="Android",
                    ListItemSystemName="Android",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=16,
                    ListItemCategoryId=6,
                    ListItemName="Angular 7",
                    ListItemSystemName="Angular 7",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=17,
                    ListItemCategoryId=6,
                    ListItemName="IOS",
                    ListItemSystemName="IOS",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=18,
                    ListItemCategoryId=6,
                    ListItemName="React JS",
                    ListItemSystemName="React JS",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=19,
                    ListItemCategoryId=6,
                    ListItemName="Vue JS",
                    ListItemSystemName="Vue JS",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=20,
                    ListItemCategoryId=6,
                    ListItemName="PHP",
                    ListItemSystemName="PHP",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=21,
                    ListItemCategoryId=6,
                    ListItemName="Adonis",
                    ListItemSystemName="Adonis",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=22,
                    ListItemCategoryId=7,
                    ListItemName= ProjectRoleListItem.Owner,
                    ListItemSystemName= ProjectRoleListItem.Owner,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=23,
                    ListItemCategoryId=7,
                    ListItemName=ProjectRoleListItem.Maintainer,
                    ListItemSystemName=ProjectRoleListItem.Maintainer,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=24,
                    ListItemCategoryId=7,
                    ListItemName=ProjectRoleListItem.Member,
                    ListItemSystemName=ProjectRoleListItem.Member,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=25,
                    ListItemCategoryId=8,
                    ListItemName=UserRoleListItem.Admin,
                    ListItemSystemName=UserRoleListItem.Admin,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=26,
                    ListItemCategoryId=8,
                    ListItemName=UserRoleListItem.ProjectLead,
                    ListItemSystemName=UserRoleListItem.ProjectLead,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=27,
                    ListItemCategoryId=8,
                    ListItemName=UserRoleListItem.SeniorQA,
                    ListItemSystemName=UserRoleListItem.SeniorQA,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=28,
                    ListItemCategoryId=8,
                    ListItemName=UserRoleListItem.Onsite,
                    ListItemSystemName=UserRoleListItem.Onsite,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                }
                 ,
                 new ListItem
                {
                    ListItemId=29,
                    ListItemCategoryId=8,
                    ListItemName=UserRoleListItem.UserMember,
                    ListItemSystemName=UserRoleListItem.UserMember,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                  new ListItem
                {
                    ListItemId=30,
                    ListItemCategoryId=9,
                    ListItemName="Module",
                    ListItemSystemName="Module",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                  new ListItem
                {
                    ListItemId=31,
                    ListItemCategoryId=9,
                    ListItemName="Function",
                    ListItemSystemName="Function",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                   new ListItem
                {
                    ListItemId=32,
                    ListItemCategoryId=9,
                    ListItemName="TestCase",
                    ListItemSystemName="TestCase",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },  new ListItem
                {
                    ListItemId=33,
                    ListItemCategoryId=10,
                    ListItemName="Automation",
                    ListItemSystemName="Automation",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                new ListItem
                {
                    ListItemId=34,
                    ListItemCategoryId=10,
                    ListItemName="Manual",
                    ListItemSystemName="Manual",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                new ListItem
                {
                    ListItemId=35,
                    ListItemCategoryId=11,
                    ListItemName=ThemeListItem.DarkMode,
                    ListItemSystemName=ThemeListItem.DarkMode,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                }, new ListItem
                {
                    ListItemId=36,
                    ListItemCategoryId=11,
                    ListItemName=ThemeListItem.LightMode,
                    ListItemSystemName=ThemeListItem.LightMode,
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },new ListItem
                {
                    ListItemId=37,
                    ListItemCategoryId=12,
                    ListItemName="Folder",
                    ListItemSystemName="Folder",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },new ListItem
                {
                    ListItemId=38,
                    ListItemCategoryId=12,
                    ListItemName="TestPlan",
                    ListItemSystemName="TestPlan",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                new ListItem
                {
                    ListItemId=39,
                    ListItemCategoryId=13,
                    ListItemName="Windows",
                    ListItemSystemName="Windows",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                 new ListItem
                {
                    ListItemId=40,
                    ListItemCategoryId=13,
                    ListItemName="Linux",
                    ListItemSystemName="Linux",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
               new ListItem
                {
                    ListItemId=41,
                    ListItemCategoryId=13,
                    ListItemName="MacOs",
                    ListItemSystemName="MacOs",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
                new ListItem
                {
                    ListItemId=42,
                    ListItemCategoryId=14,
                    ListItemName="Passed",
                    ListItemSystemName="Passed",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
              new ListItem
                {
                    ListItemId=43,
                    ListItemCategoryId=14,
                    ListItemName="Failed",
                    ListItemSystemName="Failed",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
             new ListItem
                {
                    ListItemId=44,
                    ListItemCategoryId=14,
                    ListItemName="Blocked",
                    ListItemSystemName="Blocked",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
              new ListItem
                {
                    ListItemId=45,
                    ListItemCategoryId=14,
                    ListItemName="Pending",
                    ListItemSystemName="Pending",
                    InsertPersonId=1,
                    InsertDate=DateTimeOffset.UtcNow,
                    UpdatePersonId=1,
                    UpdateDate=DateTimeOffset.UtcNow,
                    IsSystemConfig=true

                },
               
             };
            var existingListItemsinDb = context.ListItem.Select(x => x.ListItemSystemName).ToList();
            await context.ListItem.AddRangeAsync(listItems.Where(x => !existingListItemsinDb.Contains(x.ListItemSystemName)));
            await context.SaveChangesAsync();
            #endregion

            #region Person
            if (!context.Person.Any(x => x.UserName == SystemUser.UserName))
            {
                var person = new Person
                {
                    Name = SystemUser.Name,
                    UserName = SystemUser.UserName,
                    Email = SystemUser.Email,
                    InsertDate = DateTimeOffset.UtcNow,
                    UpdateDate = DateTimeOffset.UtcNow,
                    UserRoleListItemId = 25,
                    UserMarketListItemId = 6
                };
                await context.Person.AddAsync(person);
                await context.SaveChangesAsync();
            }
            #endregion
           
        }
    }
}