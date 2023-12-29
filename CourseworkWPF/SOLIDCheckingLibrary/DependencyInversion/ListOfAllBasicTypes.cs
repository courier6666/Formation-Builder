using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SOLIDCheckingLibrary
{
    static internal class ListOfAllBasicTypes
    {
        public static List<Type> allBasicCollectionTypes = new List<Type>()
        {
            // System.Collections Namespace
            typeof(ArrayList),
            typeof(BitArray),
            typeof(Hashtable),
            typeof(Queue),
            typeof(SortedList),
        
            // System.Collections.Generic Namespace
            typeof(List<>),
            typeof(Dictionary<,>),
            typeof(HashSet<>),
            typeof(Queue<>),
            typeof(Stack<>),
        
            // System.Collections.Concurrent Namespace
            typeof(ConcurrentBag<>),
            typeof(ConcurrentDictionary<,>),
            typeof(ConcurrentQueue<>),
            typeof(ConcurrentStack<>),
        
            // System.Collections.Specialized Namespace
            typeof(NameValueCollection),
        
            // System.Collections.ObjectModel Namespace
            typeof(Collection<>),
            typeof(ObservableCollection<>),
            typeof(ReadOnlyCollection<>),
            typeof(ReadOnlyDictionary<,>),
        
            // System.Collections.Immutable Namespace
            typeof(ImmutableArray<>),
            typeof(ImmutableList<>),
            typeof(ImmutableDictionary<,>),
            typeof(ImmutableHashSet<>),
            typeof(ImmutableSortedSet<>),
            typeof(ImmutableQueue<>),
            typeof(ImmutableStack<>),
        
            // System.Text.RegularExpressions Namespace
            typeof(MatchCollection),
            typeof(GroupCollection),
            typeof(CaptureCollection),
        
            // System.Net.Mail Namespace
            typeof(AlternateViewCollection),
            typeof(AttachmentCollection),
            typeof(LinkedResourceCollection),
        };
    }
}
