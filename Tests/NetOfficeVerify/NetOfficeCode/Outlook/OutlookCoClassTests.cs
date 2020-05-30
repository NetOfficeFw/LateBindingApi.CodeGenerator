﻿using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode.Outlook
{
    [TestFixture]
    public class OutlookCoClassTests : ProjectTestContext
    {
        public OutlookCoClassTests()
        : base("Outlook")
        {
        }

        [Test]
        [TestCase("_DocSiteControl.cs")]
        [TestCase("_DpxCtrl.cs")]
        [TestCase("_InspectorCtrl.cs")]
        [TestCase("_PageWrapCtrl.cs")]
        [TestCase("_RecipientControl.cs")]
        [TestCase("Account.cs")]
        [TestCase("AccountRuleCondition.cs")]
        [TestCase("Accounts.cs")]
        [TestCase("AccountSelector.cs")]
        [TestCase("AddressRuleCondition.cs")]
        [TestCase("Application.cs")]
        [TestCase("AppointmentItem.cs")]
        [TestCase("AssignToCategoryRuleAction.cs")]
        [TestCase("AttachmentSelection.cs")]
        [TestCase("AutoFormatRule.cs")]
        [TestCase("AutoFormatRules.cs")]
        [TestCase("BusinessCardView.cs")]
        [TestCase("CalendarModule.cs")]
        [TestCase("CalendarSharing.cs")]
        [TestCase("CalendarView.cs")]
        [TestCase("CardView.cs")]
        [TestCase("Categories.cs")]
        [TestCase("Category.cs")]
        [TestCase("CategoryRuleCondition.cs")]
        [TestCase("Column.cs")]
        [TestCase("ColumnFormat.cs")]
        [TestCase("Columns.cs")]
        [TestCase("ContactItem.cs")]
        [TestCase("ContactsModule.cs")]
        [TestCase("Conversation.cs")]
        [TestCase("ConversationHeader.cs")]
        [TestCase("DataSourceObject.cs")]
        [TestCase("DistListItem.cs")]
        [TestCase("DocumentItem.cs")]
        [TestCase("DoNotUseMeFolder.cs")]
        [TestCase("ExchangeDistributionList.cs")]
        [TestCase("ExchangeUser.cs")]
        [TestCase("Explorer.cs")]
        [TestCase("Explorers.cs")]
        [TestCase("Folder.cs")]
        [TestCase("Folders.cs")]
        [TestCase("FormNameRuleCondition.cs")]
        [TestCase("FormRegion.cs")]
        [TestCase("FormRegionStartup.cs")]
        [TestCase("FromRssFeedRuleCondition.cs")]
        [TestCase("IconView.cs")]
        [TestCase("ImportanceRuleCondition.cs")]
        [TestCase("Inspector.cs")]
        [TestCase("Inspectors.cs")]
        [TestCase("Items.cs")]
        [TestCase("JournalItem.cs")]
        [TestCase("JournalModule.cs")]
        [TestCase("MailItem.cs")]
        [TestCase("MailModule.cs")]
        [TestCase("MarkAsTaskRuleAction.cs")]
        [TestCase("MeetingItem.cs")]
        [TestCase("MobileItem.cs")]
        [TestCase("MoveOrCopyRuleAction.cs")]
        [TestCase("NameSpace.cs")]
        [TestCase("NavigationFolder.cs")]
        [TestCase("NavigationFolders.cs")]
        [TestCase("NavigationGroup.cs")]
        [TestCase("NavigationGroups.cs")]
        [TestCase("NavigationModule.cs")]
        [TestCase("NavigationModules.cs")]
        [TestCase("NavigationPane.cs")]
        [TestCase("NewItemAlertRuleAction.cs")]
        [TestCase("NoteItem.cs")]
        [TestCase("NotesModule.cs")]
        [TestCase("OlkBusinessCardControl.cs")]
        [TestCase("OlkCategory.cs")]
        [TestCase("OlkComboBox.cs")]
        [TestCase("OlkCommandButton.cs")]
        [TestCase("OlkContactPhoto.cs")]
        [TestCase("OlkDateControl.cs")]
        [TestCase("OlkFrameHeader.cs")]
        [TestCase("OlkCheckBox.cs")]
        [TestCase("OlkInfoBar.cs")]
        [TestCase("OlkLabel.cs")]
        [TestCase("OlkListBox.cs")]
        [TestCase("OlkOptionButton.cs")]
        [TestCase("OlkPageControl.cs")]
        [TestCase("OlkSenderPhoto.cs")]
        [TestCase("OlkTextBox.cs")]
        [TestCase("OlkTimeControl.cs")]
        [TestCase("OlkTimeZoneControl.cs")]
        [TestCase("OrderField.cs")]
        [TestCase("OrderFields.cs")]
        [TestCase("OutlookBarGroups.cs")]
        [TestCase("OutlookBarPane.cs")]
        [TestCase("OutlookBarShortcuts.cs")]
        [TestCase("PeopleView.cs")]
        [TestCase("PlaySoundRuleAction.cs")]
        [TestCase("PostItem.cs")]
        [TestCase("PreviewPane.cs")]
        [TestCase("PropertyAccessor.cs")]
        [TestCase("Reminder.cs")]
        [TestCase("Reminders.cs")]
        [TestCase("RemoteItem.cs")]
        [TestCase("ReportItem.cs")]
        [TestCase("Results.cs")]
        [TestCase("Row.cs")]
        [TestCase("Rule.cs")]
        [TestCase("RuleAction.cs")]
        [TestCase("RuleActions.cs")]
        [TestCase("RuleCondition.cs")]
        [TestCase("RuleConditions.cs")]
        [TestCase("Rules.cs")]
        [TestCase("SelectNamesDialog.cs")]
        [TestCase("SenderInAddressListRuleCondition.cs")]
        [TestCase("SendRuleAction.cs")]
        [TestCase("SharingItem.cs")]
        [TestCase("SimpleItems.cs")]
        [TestCase("SolutionsModule.cs")]
        [TestCase("StorageItem.cs")]
        [TestCase("Store.cs")]
        [TestCase("Stores.cs")]
        [TestCase("SyncObject.cs")]
        [TestCase("Table.cs")]
        [TestCase("TableView.cs")]
        [TestCase("TaskItem.cs")]
        [TestCase("TaskRequestAcceptItem.cs")]
        [TestCase("TaskRequestDeclineItem.cs")]
        [TestCase("TaskRequestItem.cs")]
        [TestCase("TaskRequestUpdateItem.cs")]
        [TestCase("TasksModule.cs")]
        [TestCase("TextRuleCondition.cs")]
        [TestCase("ThreadView.cs")]
        [TestCase("TimelineView.cs")]
        [TestCase("TimeZone.cs")]
        [TestCase("TimeZones.cs")]
        [TestCase("ToOrFromRuleCondition.cs")]
        [TestCase("UserDefinedProperties.cs")]
        [TestCase("UserDefinedProperty.cs")]
        [TestCase("ViewField.cs")]
        [TestCase("ViewFields.cs")]
        [TestCase("ViewFont.cs")]
        [TestCase("Views.cs")]
        public void CoClasses_VerifyDelegates(string classFilename)
        {
            // Arrange & Act & Assert
            this.VerifyCoClassDelegatesInFile(classFilename);
        }

        [TestCaseSource(nameof(ProjectCoClassFiles), new object[] { "Outlook" })]
        public void ClassesWithEventBindingInterface_EventBindingRegion_VerifyTheImplementationMatchesGoldFiles(string classFilename)
        {
            // Arrange
            var goldFile = Path.Combine(this.GoldProjectDir, "Classes", classFilename);
            var generatedFile = Path.Combine(this.GeneratedProjectDir, "Classes", classFilename);

            // Act
            var expected = GetEventBindingRegionFromFile(goldFile);
            var actual = GetEventBindingRegionFromFile(generatedFile);

            // Assert
            AssertDiff(expected, actual, goldFile, generatedFile, $"IEventBinding region in file {this.ProjectName}\\Classes\\{classFilename} does not match the gold file definition.");
        }
    }
}
