const ensureEndWith = function (str: string, suffix: string) {
  str = str.trim();
  suffix = suffix.trim();

  if (str.length < suffix.length) {
    return `${str}${suffix}`;
  }

  for (let i = 0; i < suffix.length; i++) {
    if (str[str.length - 1 - i] !== suffix[suffix.length - 1 - i]) {
      return `${str}${suffix}`;
    }
  }

  return str;
};

const ensureStartWith = function (str: string, prefix: string) {
  str = str.trim();
  prefix = prefix.trim();
  if (str.length < prefix.length) {
    return `${prefix}${str}`;
  }

  for (let i = 0; i < prefix.length; i++) {
    if (str[i] !== prefix[i]) {
      return `${prefix}${str}`;
    }
  }

  return str;
};

const ensureEndWithout = function (str: string, suffix: string) {
  str = ensureEndWith(str, suffix);

  return str.slice(0, -suffix.length);
};

const ensureStartWithout = function (str: string, prefix: string) {
  str = ensureEndWith(str, prefix);

  return str.slice(prefix.length, -1);
};

const normalizePath = function (path: string) {
  return path.replace(new RegExp(/\\/g), '/');
};

export {
  ensureEndWith,
  ensureStartWith,
  ensureEndWithout,
  ensureStartWithout,
  normalizePath,
};
