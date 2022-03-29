using orcwebApiSamples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace orcwebApiSamples.Api.ess_submit_proposal
{
    /// <summary>
    /// WebApi Sample Class
    /// Employee submits proposal for company
    /// </summary>
    public class ess_submit_proposalController : ApiController
    {

        /// <summary>
        /// Entry point method from Orchestra
        /// </summary>
        /// <param name="orcScreenData"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public IHttpActionResult Post([FromBody]List<OrcScreenModel> orcScreenData)
        {

            string _event = orcScreenData.Where(o => o.FieldName == "FormEvent" && o.Command == "Std").FirstOrDefault().Data;

            if (_event == "GET")
                orcScreenData = Form_Loading(orcScreenData);
            else if (_event == "POST")
                orcScreenData = Form_Action(orcScreenData);


            return Ok(orcScreenData);

        }


        /// <summary>
        /// Form loading event
        /// </summary>
        /// <param name="orcScreenData"></param>
        /// <returns></returns>
        private List<OrcScreenModel> Form_Loading(List<OrcScreenModel> orcScreenData)
        {
            //add items to select box
            orcScreenData.Add(new OrcScreenModel { Command = "Add_Item", FieldName = "CATEGORY", Data = "01-Workplace Issues" });
            orcScreenData.Add(new OrcScreenModel { Command = "Add_Item", FieldName = "CATEGORY", Data = "02-Dining Hall" });
            orcScreenData.Add(new OrcScreenModel { Command = "Add_Item", FieldName = "CATEGORY", Data = "03-Company Services" });


            int wiid = int.Parse(orcScreenData.Where(o => o.FieldName == "WIID" && o.Command == "Std").Select(s => s.Data).FirstOrDefault().ToString());

            if(wiid == 0)
            {
                //set default value
                orcScreenData.Add(new OrcScreenModel { Command = "Set_Value", FieldName = "CATEGORY", Data = "01" });
            }


            return orcScreenData;
        }



        /// <summary>
        /// When user press button on form
        /// </summary>
        /// <param name="orcScreenData"></param>
        /// <returns></returns>
        private List<OrcScreenModel> Form_Action(List<OrcScreenModel> orcScreenData)
        {
            //get return field
            var ReturnField = orcScreenData.Where(o => o.FieldName == "Return" && o.Command == "Std").FirstOrDefault();
            ReturnField.Data = "";

            //get message field
            var MessageField = orcScreenData.Where(o => o.FieldName == "Message" && o.Command == "Std").FirstOrDefault();
            MessageField.Data = "";

            //get important fields for validation
           var subjectField = orcScreenData.Where(o => o.Command == "FormData" && o.FieldName == "SUBJECT").FirstOrDefault();
           var proposalField = orcScreenData.Where(o => o.Command == "FormData" && o.FieldName == "PROPOSAL").FirstOrDefault();
           var categoryField = orcScreenData.Where(o => o.Command == "FormData" && o.FieldName == "CATEGORY").FirstOrDefault();


            //validate form 
            if(subjectField==null)
            {
                ReturnField.Data = "false";
                orcScreenData.Add(new OrcScreenModel { Command = "Set_Error", FieldName = "SUBJECT" });
            }
            else
            {
                if (string.IsNullOrEmpty(subjectField.Data))
                {
                    ReturnField.Data = "false";
                    orcScreenData.Add(new OrcScreenModel { Command = "Set_Error", FieldName = "SUBJECT" });
                }

            }

            if (proposalField == null)
            {
                ReturnField.Data = "false";
                orcScreenData.Add(new OrcScreenModel { Command = "Set_Error", FieldName = "PROPOSAL" });
            }
            else
            {
                if (string.IsNullOrEmpty(proposalField.Data))
                {
                    ReturnField.Data = "false";
                    orcScreenData.Add(new OrcScreenModel { Command = "Set_Error", FieldName = "PROPOSAL" });
                }

            }

            if (categoryField == null)
            {
                ReturnField.Data = "false";
                orcScreenData.Add(new OrcScreenModel { Command = "Set_Error", FieldName = "CATEGORY" });
            }

            else
            {
                if (string.IsNullOrEmpty(categoryField.Data))
                {
                    ReturnField.Data = "false";
                    orcScreenData.Add(new OrcScreenModel { Command = "Set_Error", FieldName = "CATEGORY" });
                }

            }


            //check if validated
            if(string.IsNullOrEmpty(ReturnField.Data))
            {
                //button pressed
                string buttonPressed = orcScreenData.Where(o => o.FieldName == "Taskname" && o.Command == "Std").FirstOrDefault().Data;

                if (buttonPressed == "DOSUBMIT")
                {
                    orcScreenData.Add(new OrcScreenModel { Command = "Clear_Agents" });
                    orcScreenData.Add(new OrcScreenModel { Command = "Set_Agent", Param1 = "P", Param2 = "1005" });
                }
            }
            else
            {
                //If not validated
                if (!string.IsNullOrEmpty(ReturnField.Data))
                    MessageField.Data = "Check your entries";
            }






            return orcScreenData;
        }





    }
}
