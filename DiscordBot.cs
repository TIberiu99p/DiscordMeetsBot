using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot
{
    public class StudyGroupsModule : ModuleBase<SocketCommandContext>
    {
        // Set up a dictionary to store the study groups
        private static Dictionary<string, (string subject, List<string> members)> studyGroups = new Dictionary<string, (string, List<string>)>();

        // Set up a command to create a new study group
        [Command("create-study-group")]
        public async Task CreateStudyGroupAsync(string groupName, string subject)
        {
            // Check if the group already exists
            if (studyGroups.ContainsKey(groupName))
            {
                await Context.Channel.SendMessageAsync($"A study group with the name {groupName} already exists!");
                return;
            }

            // Create a new study group and add it to the dictionary
            studyGroups[groupName] = (subject, new List<string> { Context.User.Mention });
            await Context.Channel.SendMessageAsync($"Study group {groupName} for {subject} created successfully!");
        }

        // Set up a command to join an existing study group
        [Command("join-study-group")]
        public async Task JoinStudyGroupAsync(string groupName)
        {
            // Check if the group exists
            if (!studyGroups.ContainsKey(groupName))
            {
                await Context.Channel.SendMessageAsync($"There is no study group with the name {groupName}!");
                return;
            }

            // Add the user to the group's member list
            studyGroups[groupName].members.Add(Context.User.Mention);
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} has joined study group {groupName}!");
        }

        // Set up a command to list all available study groups
        [Command("list-study-groups")]
        public async Task ListStudyGroupsAsync()
        {
            // Check if there are any study groups
            if (!studyGroups.Any())
            {
                await Context.Channel.SendMessageAsync("There are no study groups at the moment!");
                return;
            }

            // List all the study groups and their subjects
            var messageBuilder = new StringBuilder("Here are the available study groups:\n");
            foreach (var (groupName, (subject, _)) in studyGroups)
            {
                messageBuilder.AppendLine($"{groupName} - {subject}");
            }
            await Context.Channel.SendMessageAsync(messageBuilder.ToString());
        }

        // Set up a command to display the members of a study group
        [Command("study-group-members")]
        public async Task StudyGroupMembersAsync(string groupName)
        {
            // Check if the group exists
            if (!studyGroups.ContainsKey(groupName))
            {
                await Context.Channel.SendMessageAsync($"There is no study group with the name {groupName}!");
                return;
            }

            // Display the members of the group
            var messageBuilder = new StringBuilder($"Members of study group {groupName}:\n");
            foreach (var member in studyGroups[groupName].members)
            {
                messageBuilder.AppendLine(member);
            }
            await Context.Channel.SendMessageAsync(messageBuilder.ToString());
        }

        // Set up a command to delete a study group
        [Command("delete-study-group")]
        public async Task DeleteStudyGroupAsync(string groupName)
        {
            // Check if the group exists
            if (!studyGroups.ContainsKey(groupName))
            {
                await Context.Channel.SendMessageAsync($"There is no study group with the name {groupName}!");
                return;
            }

            // Check if the user is the owner of the group
            if (Context.User.Mention != studyGroups[groupName].members[0])
            {
                await Context.Channel.SendMessageAsync("Only the owner of the study group can delete it!");
                return;
            }

            // Remove the group from the dictionary
            studyGroups.Remove(groupName);
            await Context.Channel.SendMessageAsync($"Study group {groupName} deleted successfully!");
        }
    }
}
