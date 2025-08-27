// Stream Abstraction for Type-Agnostic Processing
// Inspired by Scheme streams and functional programming principles

/**
 * Generic stream interface that abstracts over different stream implementations
 * This allows us to write code that works with arrays, async generators, Effect streams, etc.
 */
export interface Stream<T> {
  /**
   * Process each item in the stream
   * Returns a new stream with transformed items
   */
  map<U>(fn: (item: T) => U): Stream<U>;

  /**
   * Filter items in the stream
   * Returns a new stream with only matching items
   */
  filter(predicate: (item: T) => boolean): Stream<T>;

  /**
   * Reduce/fold over the stream
   * Returns the accumulated result
   */
  reduce<U>(fn: (accumulator: U, item: T) => U, initial: U): Promise<U>;

  /**
   * Collect all items into an array
   * Useful for testing or when you need all items at once
   */
  toArray(): Promise<T[]>;

  /**
   * Process items one at a time
   * Useful for side effects or when you don't need to collect results
   */
  forEach(fn: (item: T) => void | Promise<void>): Promise<void>;

  /**
   * Take only the first N items
   */
  take(n: number): Stream<T>;

  /**
   * Skip the first N items
   */
  skip(n: number): Stream<T>;
}

/**
 * Array-based stream implementation
 * Simple implementation for testing and small datasets
 */
export class ArrayStream<T> implements Stream<T> {
  constructor(private items: T[]) {}

  map<U>(fn: (item: T) => U): Stream<U> {
    return new ArrayStream(this.items.map(fn));
  }

  filter(predicate: (item: T) => boolean): Stream<T> {
    return new ArrayStream(this.items.filter(predicate));
  }

  async reduce<U>(fn: (accumulator: U, item: T) => U, initial: U): Promise<U> {
    return this.items.reduce(fn, initial);
  }

  async toArray(): Promise<T[]> {
    return [...this.items];
  }

  async forEach(fn: (item: T) => void | Promise<void>): Promise<void> {
    for (const item of this.items) {
      await fn(item);
    }
  }

  take(n: number): Stream<T> {
    return new ArrayStream(this.items.slice(0, n));
  }

  skip(n: number): Stream<T> {
    return new ArrayStream(this.items.slice(n));
  }
}

/**
 * Async generator stream implementation
 * For real streaming with async generators
 */
export class AsyncGeneratorStream<T> implements Stream<T> {
  constructor(private generator: AsyncGenerator<T>) {}

  map<U>(fn: (item: T) => U): Stream<U> {
    // This is a simplified implementation - in practice you'd want lazy evaluation
    return new AsyncGeneratorStream(this.mapGenerator(fn));
  }

  filter(predicate: (item: T) => boolean): Stream<T> {
    return new AsyncGeneratorStream(this.filterGenerator(predicate));
  }

  async reduce<U>(fn: (accumulator: U, item: T) => U, initial: U): Promise<U> {
    let accumulator = initial;
    for await (const item of this.generator) {
      accumulator = fn(accumulator, item);
    }
    return accumulator;
  }

  async toArray(): Promise<T[]> {
    const items: T[] = [];
    for await (const item of this.generator) {
      items.push(item);
    }
    return items;
  }

  async forEach(fn: (item: T) => void | Promise<void>): Promise<void> {
    for await (const item of this.generator) {
      await fn(item);
    }
  }

  take(n: number): Stream<T> {
    return new AsyncGeneratorStream(this.takeGenerator(n));
  }

  skip(n: number): Stream<T> {
    return new AsyncGeneratorStream(this.skipGenerator(n));
  }

  private async *mapGenerator<U>(fn: (item: T) => U): AsyncGenerator<U> {
    for await (const item of this.generator) {
      yield fn(item);
    }
  }

  private async *filterGenerator(predicate: (item: T) => boolean): AsyncGenerator<T> {
    for await (const item of this.generator) {
      if (predicate(item)) {
        yield item;
      }
    }
  }

  private async *takeGenerator(n: number): AsyncGenerator<T> {
    let count = 0;
    for await (const item of this.generator) {
      if (count >= n) break;
      yield item;
      count++;
    }
  }

  private async *skipGenerator(n: number): AsyncGenerator<T> {
    let count = 0;
    for await (const item of this.generator) {
      if (count >= n) {
        yield item;
      }
      count++;
    }
  }
}

/**
 * Factory functions for creating streams
 */
export const Stream = {
  /**
   * Create a stream from an array
   */
  fromArray<T>(items: T[]): Stream<T> {
    return new ArrayStream(items);
  },

  /**
   * Create a stream from an async generator
   */
  fromAsyncGenerator<T>(generator: AsyncGenerator<T>): Stream<T> {
    return new AsyncGeneratorStream(generator);
  },

  /**
   * Create a stream from a regular generator
   */
  fromGenerator<T>(generator: Generator<T>): Stream<T> {
    return new ArrayStream([...generator]);
  },

  /**
   * Create an empty stream
   */
  empty<T>(): Stream<T> {
    return new ArrayStream([]);
  }
};
