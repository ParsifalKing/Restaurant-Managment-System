namespace Domain.Constants;

public static class Permissions
{
    public static List<string> GeneratePermissionsForModule(string module)
    {
        return
        [
            $"Permissions.{module}.Create",
            $"Permissions.{module}.View",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete"
        ];
    }

    public static class Dish
    {
        public const string View = "Permissions.Dish.View";
        public const string Create = "Permissions.Dish.Create";
        public const string Edit = "Permissions.Dish.Edit";
        public const string Delete = "Permissions.Dish.Delete";
    }

    public static class Menu
    {
        public const string View = "Permissions.Menu.View";
        public const string Create = "Permissions.Menu.Create";
        public const string Edit = "Permissions.Menu.Edit";
        public const string Delete = "Permissions.Menu.Delete";
    }

    public static class Payment
    {
        public const string View = "Permissions.Payment.View";
        public const string Create = "Permissions.Payment.Create";
        public const string Edit = "Permissions.Payment.Edit";
        public const string Delete = "Permissions.Payment.Delete";
    }

    public static class Reservation
    {
        public const string View = "Permissions.Reservation.View";
        public const string Create = "Permissions.Reservation.Create";
        public const string Edit = "Permissions.Reservation.Edit";
        public const string Delete = "Permissions.Reservation.Delete";
    }

    public static class Table
    {
        public const string View = "Permissions.Table.View";
        public const string Create = "Permissions.Table.Create";
        public const string Edit = "Permissions.Table.Edit";
        public const string Delete = "Permissions.Table.Delete";
    }

    public static class Role
    {
        public const string View = "Permissions.Role.View";
        public const string Create = "Permissions.Role.Create";
        public const string Edit = "Permissions.Role.Edit";
        public const string Delete = "Permissions.Role.Delete";
    }

    public static class User
    {
        public const string View = "Permissions.User.View";
        public const string Create = "Permissions.User.Create";
        public const string Edit = "Permissions.User.Edit";
        public const string Delete = "Permissions.User.Delete";
    }

    public static class UserRole
    {
        public const string View = "Permissions.UserRole.View";
        public const string Create = "Permissions.UserRole.Create";
        public const string Edit = "Permissions.UserRole.Edit";
        public const string Delete = "Permissions.UserRole.Delete";
    }

}
