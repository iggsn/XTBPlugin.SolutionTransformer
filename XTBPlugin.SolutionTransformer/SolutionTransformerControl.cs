using McTools.Xrm.Connection;

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using XrmToolBox.Extensibility;

namespace XTBPlugin.SolutionTransformer
{
    public partial class SolutionTransformerControl : PluginControlBase
    {
        private Settings mySettings;

        public Dictionary<string, Entity> SolutionEntities { get; set; } = new Dictionary<string, Entity>();
        public Dictionary<Guid, Entity> PublisherEntities { get; set; } = new Dictionary<Guid, Entity>();
        public Dictionary<Guid, Entity> WebResources { get; set; } = new Dictionary<Guid, Entity>();

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
            ExecuteMethod(GetWebResources);
        }

        private void LoadSolutions()
        {
            cB_Solutions.Items.Clear();

            QueryExpression solutionsQuery = new QueryExpression("solution");
            solutionsQuery.ColumnSet = new ColumnSet(true);
            solutionsQuery.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);

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

                            cB_Solutions.Items.Add(solution.GetAttributeValue<string>("uniquename"), CheckState.Checked);
                        }
                    }
                },

            });
        }

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
                        PublisherEntities = result.Entities.ToList().ToDictionary(x => x.Id);
                        foreach (Entity publisher in result.Entities)
                        {
                            if (string.IsNullOrEmpty(publisher.GetAttributeValue<string>("customizationprefix")) || clbPublisher.Items.Contains(publisher.GetAttributeValue<string>("customizationprefix")))
                                continue;

                            clbPublisher.Items.Add(publisher.GetAttributeValue<string>("customizationprefix"));
                        }
                    }
                },

            });
        }

        private void GetWebResources()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting WebResources",
                Work = (worker, args) =>
                {
                    QueryExpression webResources = new QueryExpression("webresource")
                    {
                        TopCount = 50,
                        ColumnSet = new ColumnSet("componentstate", "ishidden", "iscustomizable", "ismanaged", "webresourcetype", "name"),
                        Criteria =
                        {
                            Conditions = {
                                new ConditionExpression("ishidden", ConditionOperator.Equal, false),
                                new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                new ConditionExpression("name", ConditionOperator.DoesNotBeginWith,  "cc_shared/")
                            }
                        }
                    };

                    args.Result = Service.RetrieveMultiple(webResources);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        WebResources = result.Entities.ToList().ToDictionary(x => x.Id);
                        MessageBox.Show($"Found {result.Entities.Count} resources");
                    }
                }
            });
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Add To Solution",
                Work = (worker, args) =>
                {
                    foreach (var webResource in WebResources)
                    {
                        AddSolutionComponentRequest addSolutionComponent = new AddSolutionComponentRequest
                        {
                            AddRequiredComponents = false,
                            ComponentId = webResource.Key,
                            ComponentType = 61,
                            SolutionUniqueName = "XrmToolBoxTest"
                        };

                        Service.Execute(addSolutionComponent);
                    }

                    //1d866466-a8be-ea11-a812-000d3a378a3a

                    args.Result = true;
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    //var result = args.Result as bool;
                    //if (result != null)
                    //{
                    //    WebResources = result.Entities.ToList().ToDictionary(x => x.Id);
                    MessageBox.Show($"Updated Solution");
                    //}
                }
            });
        }

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
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
    }
}