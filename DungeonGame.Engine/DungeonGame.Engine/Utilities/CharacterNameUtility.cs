using System;
using System.Linq;

namespace DungeonGame.Engine.Utilities
{
    public class CharacterNameUtility
    {
        private static string[] MaleNames = { 
            "Arthur", "Alfred", "Abel", "Ace", "Alf", "Arlo", "Ash", "Ben", "Bob", "Bill", 
            "Brad", "Bud", "Bert", "Bart", "Baz", "Biff", "Buck", "Bo", "Bear", "Beau", 
            "Callum", "Cameron", "Carl", "Chad", "Chip", "Chuck", "Cliff", "Clint", "Clem", "Cole", 
            "Cory", "Curt", "Cy", "Dan", "Dave", "Don", "Doug", "Drew", "Dax", "Dean", 
            "Dex", "Dick", "Dolph", "Duff", "Duke", "Earl", "Edward", "Eli", "Edgy", "Erb", 
            "Fred", "Frank", "Finn", "Fritz", "Floyd", "Fletch", "Gabe", "Gus", "Guy", "Gary", 
            "Gale", "Gene", "Glen", "Gil", "Gord", "Hank", "Hal", "Herb", "Hugh", "Hugo", 
            "Ike", "Ian", "Isaac", "Ivan", "Joe", "Jeff", "Jim", "Jack", "John", "Jake", 
            "Jerry", "Jude", "Jeb", "Jed", "Jess", "Jett", "Jinx", "Jones", "Judd", "Ken", "Kurt", 
            "Karl", "Kev", "Kip", "Kirk", "Kyle", "Kane", "Kobe", "Len", "Lou", "Lee", 
            "Leo", "Lyle", "Linc", "Lance", "Lars", "Leif", "Max", "Matt", "Mike", "Mitch", 
            "Moe", "Milt", "Mack", "Mick", "Mel", "Mort", "Nate", "Ned", "Neil", "Nick", 
            "Niles", "Nash", "Norm", "Noah", "Otis", "Oz", "Oscar", "Oat", "Pat", "Paul", 
            "Phil", "Pete", "Puck", "Pike", "Quinn", "Quent", "Ray", "Rich", "Rob", "Rick", 
            "Ron", "Russ", "Ralph", "Rex", "Rhett", "Rudy", "Reed", "Reid", "Sam", "Stan", 
            "Sid", "Stanley", "Sal", "Seth", "Saul", "Sly", "Skip", "Smith", "Spud", "Scott", "Sean",
            "Steve", "Tom", "Tim", "Ted", "Todd", "Tex", "Ty", "Tate", "Toby", "Trent", "Troy", 
            "Vic", "Vern", "Vince", "Van", "Vic", "Walt", "Wesley", "Will", "Wade", "Ward", 
            "Wyatt", "Wolf", "Zack", "Zeke", "Zane", "Zeb", "Zoy", "Alfie", "Bernie", 
            "Burt", "Chet", "Deke", "Ernie", "Fritz", "Gordie", "Horton", "Irwin", 
            "Jules", "Kelvin", "Lester", "Monty", "Nigel", "Oscar", "Percy", "Quig", 
            "Rolly", "Sully", "Tully", "Upton", "Vinnie", "Wally", "Yancy", "Zeff"
        };

        private static string[] FemaleNames = { 
            "Ann", "Amy", "Barb", "Babs", "Beth", "Bev", "Bea", "Belle", "Bree", "Cora", 
            "Cass", "Cleo", "Deb", "Dot", "Dee", "Dora", "Dawn", "Eve", "Enid", "Edie", 
            "Flo", "Fay", "Fern", "Fran", "Gail", "Gwen", "Gia", "Hope", "Ida", "Ivy", 
            "Iris", "Jan", "Jen", "Jill", "Joy", "Jo", "June", "Jade", "Joan", "Kay", 
            "Kim", "Kat", "Kit", "Liz", "Lou", "Lynn", "Lois", "Liv", "Lana", "Lola", 
            "May", "Meg", "Mia", "Marge", "Mona", "Maud", "Nan", "Nell", "Nora", "Pam", 
            "Pat", "Peg", "Pearl", "Prue", "Rose", "Ruth", "Rae", "Rita", "Ruby", "Sue", 
            "Sess", "Stacy", "Tess", "Trish", "Trix", "Valerie", "Viv", "Vi", "Vera", "Wanda", 
            "Zoe", "Zelda", "Zita"
        };

        private static double ChanceToPickMaleName = 0.7;

        public static (string, bool) GetRandomName()
        {
            var isMaleName = RandomUtility.Random() <= ChanceToPickMaleName;

            var name = (isMaleName ? MaleNames : FemaleNames).OrderBy(_ => Guid.NewGuid()).First();

            return (name, isMaleName);
        } 
    }
}
