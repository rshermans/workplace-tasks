using WorkplaceTasks.Api.Domain.Entities;

namespace WorkplaceTasks.Api.Domain.Policies;

public static class TaskAuthorizationPolicy
{
    public static bool CanCreate(User user)
    {
        // Everyone can create tasks
        return true; 
    }

    public static bool CanDelete(User user, TaskItem task)
    {
        if (user.Role == Role.Admin) return true;
        
        if (user.Role == Role.Manager)
        {
            // Manager can delete ONLY their own tasks (as per current requirement)
            // Note: Plan says manager can delete only tasks they created.
            return task.CreatedByUserId == user.Id;
        }

        if (user.Role == Role.Member)
        {
            // Member can delete ONLY their own tasks
            return task.CreatedByUserId == user.Id;
        }

        return false;
    }

    public static bool CanUpdate(User user, TaskItem task)
    {
        if (user.Role == Role.Admin) return true;
        
        if (user.Role == Role.Manager) return true; // Manager can update any task

        if (user.Role == Role.Member)
        {
            // Member can update ONLY their own tasks
            return task.CreatedByUserId == user.Id;
        }

        return false;
    }
}
