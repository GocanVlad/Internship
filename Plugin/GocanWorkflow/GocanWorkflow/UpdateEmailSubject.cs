﻿using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Workflow.ComponentModel;

namespace GocanWorkflow
{
    public class UpdateEmailSubject:CodeActivity
    {
        // Implement a ValueStorageCollectionForWorkflow object as a dependency property, which stores the list of values to retrieve for output.
        public static DependencyProperty MultiOutputInfoProperty =
            DependencyProperty.Register("MultiOutputInfo", typeof(ValueStorageCollectionForWorkflow), typeof(CustomActivity));

        // Implement a Hashtable as a dependency property, which stores the results after processing within the workflow activity.
        public static DependencyProperty MultiOutputProperty =
            DependencyProperty.Register("MultiOutput", typeof(Hashtable), typeof(CustomActivity));;
        [RequiredArgument]
        [Input("String input")]
        public InArgument<string> FormInput { get; set; }

        [Output("String output")]
        public OutArgument<string> SubjectOutput { get; set; }

        [Output("Out table")]
        public OutArgument<string> TableOutput { get; set; }
        


        protected override void Execute(CodeActivityContext context)
        {
            string sender = FormInput.Get(context); 
            string image = "data:image/png;base64,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";

            string table = "<html><style>table, th, td {border:1px solid black;}</style><body>" +
                            "<h3>Dear Customer,</h3>" +
                            "<table style=\"width:50%\"><tr><th>1</th><th>2</th><th>3</th></tr>" +
                            " <tr><td>4</td><td>5</td><td>6</td></tr>" +
                            "<tr><td>7</td><td>8</td><td>9</td></tr>" +
                            "</table>" +
                            $"<img align =\"none\" alt=\"\" src=\"{image}\"  >" +
                            "</body></html>";

            SubjectOutput.Set(context, $"This Email was created and CC to {sender} at {DateTime.UtcNow}.");
            TableOutput.Set(context,table);
            
        }
    }
}