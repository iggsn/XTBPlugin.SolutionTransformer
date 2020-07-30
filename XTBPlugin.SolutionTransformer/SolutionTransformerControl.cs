using McTools.Xrm.Connection;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;

namespace XTBPlugin.SolutionTransformer
{
    public partial class SolutionTransformerControl : PluginControlBase, IGitHubPlugin, IStatusBarMessenger
    {
        private Settings mySettings;

        /// <summary>
        /// Contains List of all unmanaged Solutions.
        /// </summary>
        public Dictionary<string, Entity> SolutionEntities { get; set; } = new Dictionary<string, Entity>();

        /// <summary>
        /// Contains the Entity of the Selected Solution.
        /// </summary>
        private Entity TargetSolution;

        public Dictionary<Guid, Entity> PublisherEntities { get; set; } = new Dictionary<Guid, Entity>();
        public Dictionary<Guid, Entity> WebResources { get; set; } = new Dictionary<Guid, Entity>();

        #region IGitHubPlugin        
        public string RepositoryName => "XTBPlugin.SolutionTransformer";

        public string UserName => "iggsn";
        #endregion

        public SolutionBuilder solutionBuilder;

        #region IStatusBarMessenger
        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;
        #endregion

        public SolutionTransformerControl()
        {
            InitializeComponent();
        }

        private void SolutionTransformerControl_Load(object sender, EventArgs e)
        {
            //ShowInfoNotification("This is a notification that can lead to XrmToolBox repository", new Uri("https://github.com/MscrmTools/XrmToolBox"));

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            pg_Settings.SelectedObject = mySettings;

            tsbRefresh.Enabled = false;
            tsbAddToSolution.Enabled = false;

            ExecuteMethod(LoadSolutions);
            ExecuteMethod(LoadPublisher);
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbResfresh_Click(object sender, EventArgs e)
        {
            // The ExecuteMethod method handles connecting to an
            // organization if XrmToolBox is not yet connected
            ExecuteMethod(GetSolutionComponents);
        }

        /// <summary>
        /// Loads only the unmanaged Solutions.
        /// </summary>
        private void LoadSolutions()
        {
            tsbRefresh.Enabled = false;
            tsbAddToSolution.Enabled = false;
            TargetSolution = null;
            lbl_SelectedSolution.Text = "Selected Solution: -none-";
            cB_Solutions.Items.Clear();
            cB_Solutions.SelectedIndex = -1;
            cB_Solutions.Text = "";

            QueryExpression solutionsQuery = new QueryExpression("solution");
            solutionsQuery.ColumnSet = new ColumnSet("solutionid", "uniquename", "friendlyname");
            solutionsQuery.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);
            solutionsQuery.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);
            solutionsQuery.Criteria.AddCondition("parentsolutionid", ConditionOperator.Null);

            var childSolutions = solutionsQuery.AddLink("solution", "solutionid", "parentsolutionid", JoinOperator.LeftOuter);

            // Add columns to QEsolution_solution.Columns
            childSolutions.Columns.AddColumns("uniquename");
            childSolutions.LinkCriteria.AddCondition("solutionid", ConditionOperator.Null);

            solutionsQuery.AddOrder("uniquename", OrderType.Ascending);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting all unmanaged solutions",

                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(solutionsQuery);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        LogError("An error happend while loading the solutions.", args.Error);
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        SolutionEntities = result.Entities.ToList().ToDictionary(x => x.GetAttributeValue<string>("uniquename"));
                        foreach (Entity solution in result.Entities)
                        {
                            if (solution.GetAttributeValue<string>("uniquename") == "Active" || solution.GetAttributeValue<string>("uniquename") == "Default" || solution.GetAttributeValue<string>("uniquename") == "Basic")
                                continue;

                            cB_Solutions.Items.Add(solution.GetAttributeValue<string>("uniquename"));
                        }

                        // load last solution
                        if (!string.IsNullOrEmpty(mySettings.LastTargetSolutionName))
                        {
                            if (cB_Solutions.Items.Contains(mySettings.LastTargetSolutionName))
                            {
                                cB_Solutions.SelectedItem = mySettings.LastTargetSolutionName;
                                tsbRefresh.Enabled = true;
                            }
                        }
                    }
                },

            });
        }

        /// <summary>
        /// Loads all Publishers, that are not readonly and have a prefix.
        /// </summary>
        private void LoadPublisher()
        {
            cB_Solutions.Items.Clear();

            QueryExpression publisherQuery = new QueryExpression("publisher")
            {
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("isreadonly", ConditionOperator.Equal, false),
                        new ConditionExpression("customizationprefix", ConditionOperator.NotNull)
                    }
                }
            };

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting all publisher",

                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(publisherQuery);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        LogError("An error happend while loading the publishers.", args.Error);
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        LogInfo("Retreived Publishers");
                        PublisherEntities = result.Entities.ToList().ToDictionary(x => x.Id);
                        foreach (Entity publisher in result.Entities)
                        {
                            if (string.IsNullOrEmpty(publisher.GetAttributeValue<string>("customizationprefix")) || clbPublisher.Items.Contains(publisher.GetAttributeValue<string>("customizationprefix")))
                                continue;

                            clbPublisher.Items.Add(publisher.GetAttributeValue<string>("customizationprefix"), true);
                            LogInfo($"Added Publisher with prefix {publisher.GetAttributeValue<string>("customizationprefix")}");
                        }
                    }
                },
            });
        }

        private void GetSolutionComponents()
        {
            solutionBuilder = new SolutionBuilder(Service);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting Solution-Components",
                Work = (worker, args) =>
                {
                    LogInfo("Collecting Solution Items...");
                    List<string> publisher = new List<string>();
                    foreach (var item in clbPublisher.CheckedItems)
                    {
                        if (clbPublisher.GetItemCheckState(clbPublisher.Items.IndexOf(item)) == CheckState.Checked)
                        {
                            publisher.Add(item.ToString());
                        }
                    }

                    SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs("Collect Solution Components"));

                    args.Result = solutionBuilder.CollectComponents(mySettings, publisher, worker.ReportProgress);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs("Finished with Errors."));
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    bool result = (bool)args.Result;
                    if (result)
                    {
                        tsbAddToSolution.Enabled = true;
                        SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs("Finished successfully."));
                    }
                },
                ProgressChanged = e => { SetWorkingMessage(e.UserState.ToString()); }
            });
        }

        private void tsbAddToSolution_Click(object sender, EventArgs e)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Add To Solution",
                Work = (worker, args) =>
                {
                    SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs("Adding Components to Solution..."));

                    args.Result = solutionBuilder.AddComponentsToSolution(TargetSolution.GetAttributeValue<string>("uniquename"), mySettings, worker.ReportProgress);

                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs("Finished with Errors."));
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    bool result = (bool)args.Result;
                    if (result)
                    {
                        SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs("Finished successfully."));
                    }
                },
                ProgressChanged = f => { SetWorkingMessage(f.UserState.ToString()); }
            });
        }

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            SettingsManager.Instance.Save(GetType(), mySettings);
            base.ClosingPlugin(info);
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        /// <summary>
        /// Refresh the Solutions List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ReloadSolutions_Click(object sender, EventArgs e)
        {
            LoadSolutions();
        }

        private void cB_Solutions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox solutionCbx = (ComboBox)sender;

            string selectedItem = (string)solutionCbx.SelectedItem;
            TargetSolution = SolutionEntities[selectedItem];

            lbl_SelectedSolution.Text = $"Selected Solution: {TargetSolution.GetAttributeValue<string>("friendlyname")}";

            tsbRefresh.Enabled = true;

            mySettings.LastTargetSolutionName = selectedItem;
        }
    }
}