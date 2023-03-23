using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Metrics.Models
{
    /// <summary>
    /// <para><see cref="MetricTags"/> is an ordered list of key-value pairs (each pair called a <see cref="MetricTag"/>) that distinguishes one metric from another.</para>
    /// <para>Keys and values are both non-null <see cref="string">strings</see>.</para>
    /// <para>Two <see cref="MetricEvent"/>s belong to the same metric if and only if their <see cref="MetricEvent.Tags"/> are equal.</para>
    /// <para><see cref="MetricTags"/> collection is immutable and append-only.</para>
    /// </summary>
    [PublicAPI]
    public class MetricTags : IReadOnlyList<MetricTag>, IEquatable<MetricTags>
    {
        /// <summary>
        /// An instance of <see cref="MetricTags"/> with no contents.
        /// </summary>
        public static readonly MetricTags Empty = new MetricTags(Array.Empty<MetricTag>(), 0);

        private readonly MetricTag[] items;
        private readonly int hashCode;
        private int appendsDone;

        public MetricTags(int capacity)
            : this(new MetricTag[capacity], 0)
        {
        }

        public MetricTags(params MetricTag[] tags)
            : this(tags, tags.Length)
        {
        }

        private MetricTags(MetricTag[] items, int count)
        {
            this.items = items;

            Count = count;

            hashCode = items.Take(count).Aggregate(count, (hash, tag) => (hash * 397) ^ tag.GetHashCode());
        }

        public int Count { get; }

        /// <summary>
        /// <para>Appends a new <see cref="MetricTag"/> with given <paramref name="key"/> and <paramref name="value"/> and returns a new instance of <see cref="MetricTags"/> collection.</para>
        /// <para>Current instance is not modified.</para>
        /// </summary>
        [NotNull]
        public MetricTags Append([NotNull] string key, [NotNull] string value)
            => Append(new MetricTag(key, value));

        /// <summary>
        /// <para>Appends given <paramref name="tag"/> and returns a new instance of <see cref="MetricTags"/> collection.</para>
        /// <para>Current instance is not modified.</para>
        /// </summary>
        [NotNull]
        public MetricTags Append([NotNull] MetricTag tag)
            => Append(new[] {tag});

        /// <summary>
        /// <para>Appends given <paramref name="tags"/> sequence and returns a new instance of <see cref="MetricTags"/> collection.</para>
        /// <para>Current instance is not modified.</para>
        /// </summary>
        [NotNull]
        public MetricTags Append([NotNull] IReadOnlyList<MetricTag> tags)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(tags));

            if (tags.Count == 0)
                return this;

            var currentArray = items;
            var fitsIntoCurrentArray = currentArray.Length >= Count + tags.Count;
            var isFirstAppend = Interlocked.Increment(ref appendsDone) == 1;

            if (!isFirstAppend || !fitsIntoCurrentArray)
            {
                var newLength = fitsIntoCurrentArray
                    ? items.Length
                    : Math.Max(4, Math.Max(items.Length * 2, Count + tags.Count));
                currentArray = new MetricTag[newLength];
                Array.Copy(items, 0, currentArray, 0, Count);
            }

            for (var i = 0; i < tags.Count; i++)
                Interlocked.Exchange(ref currentArray[Count + i], tags[i]);

            return new MetricTags(currentArray, Count + tags.Count);
        }

        public IEnumerator<MetricTag> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return items[i];
        }

        public override string ToString() =>
            string.Join(", ", this);

        public MetricTag this[int index]
        {
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException();

                return items[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #region Equality

        public bool Equals(MetricTags other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Count != other.Count)
                return false;

            if (hashCode != other.hashCode)
                return false;

            for (var i = 0; i < Count; i++)
            {
                if (!items[i].Equals(other.items[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj) => Equals(obj as MetricTags);

        public override int GetHashCode() => hashCode;

        #endregion
    }
}