// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.
/**
 * Swap to div is necessary to skip code highlight process.
 */
$(".lang-mermaid").each(function() {
  const oldEl = $(this);
  const newEl = $("<div>");
  $.each(this.attributes, function(i, attr) { newEl.attr(attr.name, attr.value); });
  newEl.html(oldEl.html());
  oldEl.replaceWith(newEl);
});

/**
 * Swap classes for DocFx script to disable tab groups
 */
$(".tabGroup").addClass('tabGroupAlt').removeClass('tabGroup');

/**
 * Swap ids for DocFx script to disable affix generator
 */
$("#affix").attr("id", "affixAlt");

/**
 * Remove rel metadata temporarily for DocFx script to disable search function
 */
var rel = $("meta[property='docfx\\:rel']").detach();

/**
 *  Stub for DocFx script to disable code syntax highlighting
 */
window.hljs = {
  highlightBlock: (block) => {},
};

const filterKeywords = {
  "type": "api,article",
  "title": "title",
  "keyword": "keyword",
  "lang": "js,ts,cs,lua",
};

function getFilterKeywords() {
  return Object.keys(filterKeywords);
}

const filterRegex = new RegExp("(" + getFilterKeywords().join('|') + "):(\\s*\\w*(?<!\\s))", 'g');
const filterRegexFinal = new RegExp("(" + getFilterKeywords().join('|') + "):\\s*(\\w*(?<!\\s))", 'g');

const userAgent = navigator.userAgent.toLowerCase();
const mobileDevices = ['Android','webOS','iPhone','iPad','iPod','BlackBerry'];
const isMobile = mobileDevices.filter(el => userAgent.indexOf(el.toLowerCase()) > -1).length > 0;
const isAndroid = userAgent.indexOf("Android".toLowerCase()) > -1;

const darkThemeMq = window.matchMedia("(prefers-color-scheme: dark)");
switchTheme();

$.ajaxSetup({
  mimeType: "text/plain",
});

var query;
var relHref;

$("#navbar").arrive(".navbar-nav", function() {
  $("#navbar .nav").parents("li.active").addClass("in");
  $("#navbar .nav > li > .expand-stub").unbind("click").click(function(ev) {
    $(ev.target).parent().toggleClass("in");
  });
  $("#navbar .nav > li > .expand-stub + a:not([href])").unbind("click").click(function(ev) {
    $(ev.target).parent().toggleClass("in");
  });
  const parentEl = $("#sidebar .sidebar");
  const el = $("a.sidebar-item.active");
  if (!el.length) {
    return
  }
  const elOffset = el.offset().top + parentEl.scrollTop();
  const elHeight = el.height();
  parentEl.animate({
    scrollTop: elOffset - ((parentEl.height() - elHeight) / 2),
  }, 0);
});

$(window).on("load", function() {
  darkThemeMq.addEventListener("change", switchTheme());
  highlight();
  renderTabs();
  renderFlowcharts();
  enableSearch();
  $("a:not([data-tab])").off("click").on("click", delegateAnchors);
  $(".blackout").on("click", toggleMenu);
  $(".navbar-toggler").on("click", toggleMenu);
});

$(window).on("load hashchange", function() {
  scrollIfAnchor(window.location.hash);
});

window.refresh = function(article) {
  highlight();
  renderTabs();
  renderFlowcharts();
}

function switchTheme(theme) {
  const curTheme = theme || localStorage.getItem("theme") || "auto";
  let themes = ["dark", "light"];
  if ((curTheme === "auto" && !darkThemeMq.matches) || curTheme === "light") themes = themes.reverse();
  $(document.documentElement).addClass("theme-" + themes[0]);
  $(document.documentElement).removeClass("theme-" + themes[1]);
  $("#theme-menu .theme-option").removeClass("active");
  $("#theme-menu .theme-option." + curTheme).addClass("active");
  $("link[href*='highlight.js'][href*='styles'][href*='-" + themes[0] + "']").removeAttr("disabled");
  $("link[href*='highlight.js'][href*='styles'][href*='-" + themes[1] + "']").attr("disabled", "disabled");
}

function enableSearch() {
  relHref = rel.attr("content");
  if (typeof relHref === "undefined") {
    return;
  }
  try {
    var indexReady = $.Deferred();

    const worker = new Worker(relHref + "styles/search-worker.js");
    worker.onmessage = function(ev) {
      switch (ev.data.e) {
        case "index-ready":
          indexReady.resolve();
          break;
        case "query-ready":
          var hits = ev.data.d;
          handleSearchResults(hits);
          break;
      }
    }

    indexReady.promise().done(function() {
      $("#search-query").on("keydown", function(ev) {
        if (ev.originalEvent.isComposing || ev.key !== "Escape") {
            return;
        }
        ev.preventDefault();
        $("#search-query").blur();
      });

      $("#search-query").on("beforeinput textInput", function(ev) {
        if (ev.originalEvent.inputType !== "insertParagraph" && ev.originalEvent.data?.split(/(?<=[^\\])/)?.pop() !== "\n") {
          return;
        }
        ev.preventDefault();
        handleSearchInput(this);
      });
      $("body").bind("queryReady", function() {
        worker.postMessage({ q: query });
      });
      if (query && (query.length >= 3)) {
        worker.postMessage({ q: query });
      }
    });

    $(window).on("keydown", function(ev) {
      if (ev.metaKey || !ev.ctrlKey || ev.altKey || !ev.shiftKey || ev.key !== "F") {
          return;
      }
      ev.preventDefault();
      $("#search-query").focus();
    });

    $(".btn-link.search-help").on("mousedown", function(ev) {
      const btnHref = relativeUrlToAbsoluteUrl(window.location.href, "../articles/search-help.html");
      window.location.href = btnHref;
    });

    for (let i = 0; i < getFilterKeywords().length; i++) {
      const key = getFilterKeywords()[i];
      $("#search-menu").append("<div class=\"option\" data-id=\"" + i + "\"><span class=\"filter\">"
       + key + ":</span><span class=\"answer\">"
        + filterKeywords[key] + "</span></div>");
    }

    addSearchEvent();
  } catch (e) {
    console.error(e);
  }
}

function addSearchEvent() {
  $("#search-query").on("focusin", function(ev) {
    const el = $(this);
    if (el.text() !== "") {
      return;
    }
    $("#search-menu").addClass("active");
    $(".btn-link.search").toggleClass("active", isSearchInputAValidQuery(this));
  });
  $("#search-menu .option").on("mousedown", function(ev) {
    const el = $(this).find(".filter");
    if (!el.length) {
      return;
    }
    ev.preventDefault();
    const searchInput = $("#search-query");
    searchInput.text(searchInput.text() + el.text());
    insertSearchKeywords(searchInput);
    setCurrentCursorPosition(searchInput[0], searchInput.text().length);
    $("#search-menu").removeClass("active");
  });
  $("#search-query").on("focusout", function(ev) {
    $("#search-menu").removeClass("active");
  });
  $("#search-query").on("keyup", function(ev) {
    if (ev.key === "Escape") {
      return;
    }
    const el = $(ev.currentTarget);
    const prevVal = el.data("text");
    const curVal = el.text();
    $("#search-query-clear").toggleClass("active", ev.currentTarget.innerText.length > 0);
    $("#search-menu").toggleClass("active", ev.currentTarget.innerText.length == 0);
    $(".btn-link.search").toggleClass("active", isSearchInputAValidQuery(ev.currentTarget) && prevVal != curVal);
    $("#theme-menu").removeClass("active");
    $(ev.currentTarget).data("text", curVal);
  });
  $("#search-query").on("input", function(ev) {
    if (ev.originalEvent.isComposing) {
      return;
    }
    const pos = getCurrentCursorPosition(ev.currentTarget);
    insertSearchKeywords(ev.currentTarget);
    setCurrentCursorPosition(ev.currentTarget, pos);
  });
  $("body").on("searchEvent", function() {
    $("#search").removeAttr("style");
  });
  $("body").on("queryReady", function() {
    $("#search-menu").removeClass("show");
  });
  $("#pagination").on("page", function(evt, page) {
    $(".content-column").scrollTop(0);
  });
  $(".btn-link.search-back").on("click", toggleSearch);
  $(".btn-link.search-tip").on("click", function() {
    toggleSearch.call(this);
    $("#search-query").click().focus();
  });
  $(".btn-link.search").on("click", function() {
    $("#search-query").trigger($.Event("beforeinput", {originalEvent: {inputType: "insertParagraph"}, preventDefault: () => {}}));
    $(".btn-link.search").toggleClass("active");
  });
  $("#search-query-clear").on("mousedown", function(ev) {
    ev.preventDefault();
    $("#search-query").focus();
  });
  $("#search-query-clear").on("click", function(ev) {
    ev.preventDefault();
    $("#search-query").text("").trigger($.Event("keyup", {originalEvent: {inputType: "deleteContent"}, preventDefault: () => {}}));
  });
  $(".btn-link.theme").on("click", function(ev) {
    ev.preventDefault();
    $("#theme-menu").toggleClass("active");
  });
  $("#theme-menu .theme-option").on("mousedown", function(ev) {
    ev.preventDefault();
    const selTheme = $(this).attr("data-theme");
    if (selTheme === "auto") {
      localStorage.removeItem("theme");
    } else {
      localStorage.setItem("theme", selTheme);
    }
    switchTheme();
    $("#theme-menu").removeClass("active");
  });
  $(document).click(function(ev) {
    if ($(ev.target).is("#theme-menu") || $(ev.target).is(".btn-link.theme")) {
      return;
    }
    $("#theme-menu").removeClass("active");
  });
  $(window).on("resize", function() {
    const searchQuery = $("#search-query");
    if ($(this).width() >= 1024) {
      $(".btn-link.search-back").click();
    } else if (searchQuery.is(":focus") || searchQuery.text() !== "") {
      $(".btn-link.search-tip").click();
    }
  });
}

function handleSearchInput(el) {
  const str = $(el).text();
  if (str.length < 3) {
    flipContents("show");
  } else if (isSearchInputAValidQuery(el)) {
    query = str.replaceAll(filterRegexFinal, "$1:$2");
    flipContents("hide");
    $("#search-results > .search-list > span").text("\"" + str + "\"");
    $("body").trigger("queryReady");
  } else {
     flipContents("hide");
     handleSearchResults([]);
     $("#search-results > .search-list > span").text("\"" + str + "\"");
     $("#search-results > .sr-items").html("<p>Invalid search query</p>");
  }
}

function toggleSearch() {
  const el = $(this);
  const z = $(".btn-link.search-back");
  const w = $("#search");
  if (el?.hasClass("search-back")) {
    z?.removeClass("active");
    w?.removeClass("float");
    flipContents("show");
    $("#search-query").text("");
  } else if (el?.hasClass("search-tip")) {
    z?.addClass("active");
    w?.addClass("float");
  }
}

function insertSearchKeywords(el) {
  const str = $(el).text().replaceAll(filterRegex, function(match, p1, p2, offset, str) {
    const searchTerm = $("<span />").addClass("field-term").addClass(p1);
    const field = $("<span />").addClass("field").html(p1 + ':');
    searchTerm.append(field);
    if (p2 != "") {
      const term = $("<span />").addClass("term").html(p2);
      searchTerm.append(term);
    }
    return searchTerm.prop('outerHTML');
  });
  $(el).html(str);
}

function isSearchInputAValidQuery(el) {
  if (!$(el).length) {
    return false;
  }
  const str = $(el).text();
  if (!/^(?!\s).*(?<!\s)$/.test(str)) {
    return false;
  }
  const keywords = str.matchAll(filterRegex);
  for (const word of keywords) {
    if (getFilterKeywords().indexOf(word[1]) === -1 || !word[2]) {
      return false;
    }
  }
  return str.length >= 3;
}

function flipContents(action) {
  if (action === "show") {
    $(".hide-when-search").show();
    $("#search-results").hide();
  } else {
    $(".hide-when-search").hide();
    $("#search-results").show();
  }
}

function createRange(node, offset, range) {
  const ptr = isNaN(offset) ? offset : { value: offset };
  if (!range) {
    range = document.createRange();
    range.selectNode(node);
    range.setStart(node, 0);
  }
  if (ptr.value === 0) {
      range.setEnd(node, ptr.value);
  } else if (node && ptr.value > 0) {
    if (node.nodeType === Node.TEXT_NODE) {
      if (node.textContent.length < ptr.value) {
        ptr.value -= node.textContent.length;
      } else {
        range.setEnd(node, ptr.value);
        ptr.value = 0;
      }
    } else {
      for (let i = 0; i < node.childNodes.length; i++) {
        range = createRange(node.childNodes[i], ptr, range);
        if (ptr.value === 0) break;
      }
    }
  }
  return range;
}

function isChildOf(node, parentNode) {
  while (node !== null) {
    if (node.id === parentNode.id) {
      return true;
    }
    node = node.parentNode;
  }
  return false;
}

function getCurrentCursorPosition(el) {
  const selection = window.getSelection();
  if (selection.rangeCount <= 0) {
    return 0;
  }
  var range = selection.getRangeAt(0);
  var preCaretRange = range.cloneRange();
  preCaretRange.selectNodeContents(el);
  preCaretRange.setEnd(range.endContainer, range.endOffset);
  return preCaretRange.toString().length;
}

function setCurrentCursorPosition(el, offset) {
  if (offset < 0) {
    return;
  }
  const selection = window.getSelection();
  const range = createRange(el, offset);
  if (!range) {
    return;
  }
  range.collapse(false);
  selection.removeAllRanges();
  selection.addRange(range);
}

function handleSearchResults(hits) {
  var numPerPage = 10;
  var pagination = $("#pagination");
  pagination.empty();
  pagination.removeData("twbs-pagination");
  if (hits.length === 0) {
    $("#search-results > .sr-items").html("<p>No results found</p>");
  } else {        
    pagination.twbsPagination({
      first: pagination.data("first"),
      prev: pagination.data("prev"),
      next: pagination.data("next"),
      last: pagination.data("last"),
      totalPages: Math.ceil(hits.length / numPerPage),
      visiblePages: 5,
      onPageClick: function(event, page) {
        var start = (page - 1) * numPerPage;
        var curHits = hits.slice(start, start + numPerPage);
        $("#search-results > .sr-items").empty().append(
          curHits.map(function(hit) {
            var currentUrl = window.location.href;
            var itemRawHref = relativeUrlToAbsoluteUrl(currentUrl, relHref + hit.href);
            var itemHref = relHref + hit.href + "?q=" + encodeURIComponent(query);
            var itemTitle = hit.title;
            var itemBrief = extractContentBrief(hit.keywords);

            var itemNode = $("<div>").attr("class", "sr-item");
            var itemTitleNode = $("<div>").attr("class", "item-title").append($("<a>").attr("href", itemHref).attr("target", "_blank").text(itemTitle));
            var itemHrefNode = $("<div>").attr("class", "item-href").text(itemRawHref);
            var itemBriefNode = $("<div>").attr("class", "item-brief").text(itemBrief);
            itemNode.append(itemTitleNode).append(itemHrefNode).append(itemBriefNode);
            return itemNode;
          })
        );
        convertQueryIntoWords(query).forEach(function(word) {
          const options = {
            accuracy: {
              "value": "partially",
              "limiters": ":;.,-–—‒_(){}[]!'\"+=".split(""),
            },
            separateWordSearch: false,
            wildcards: "enabled",
            ignorePunctuation: ":;.,-–—‒_(){}[]!'\"+=".split(""),
          };
          if (word.startsWith("title:")) {
            $("#search-results > .sr-items .item-title").mark(word.substring(6), options);
          } else {
            $("#search-results > .sr-items *").mark(word, options);
          }
        });
      }
    });
  }
}

function convertQueryIntoWords(query) {
  return query.split(/\s+/g).map(term => {
    if (term === "" || term.startsWith('-')) {
      return null;
    }
    const keyword = term.split(':')[0];
    const hasKeyword = getFilterKeywords().indexOf(keyword) > -1;
    if (hasKeyword && keyword !== "title" && keyword !== "keyword") {
      return null;
    }
    // if (hasKeyword) { term = term.substring(keyword.length + 1); }
    return term.split('^')[0].split('~')[0].replace("+", "");
  }).filter(word => word != null);
}

function relativeUrlToAbsoluteUrl(currentUrl, relativeUrl) {
  var currentItems = currentUrl.split(/(?!\/{2})\/+(?<!\/{2})/);
  var relativeItems = relativeUrl.split(/\/+/);
  var depth = currentItems.length - 1;
  var items = [];
  for (var i = 0; i < relativeItems.length; i++) {
    if (relativeItems[i] === "..") {
      depth--;
    } else if (relativeItems[i] !== '.') {
      items.push(relativeItems[i]);
    }
  }
  return currentItems.slice(0, depth).concat(items).join('/');
}

function extractContentBrief(content) {
  var briefOffset = 512;
  var words = convertQueryIntoWords(query).filter(word => !word.startsWith("title:") && !word.startsWith("keyword:"));
  var queryIndex = content.indexOf(words[0]);
  var briefContent;
  if (queryIndex > briefOffset) {
    return "..." + content.slice(queryIndex - briefOffset, queryIndex + briefOffset) + "...";
  } else if (queryIndex <= briefOffset) {
    return content.slice(0, queryIndex + briefOffset) + "...";
  }
}

function highlight() {
  relHref = rel.attr("content");
  if (typeof relHref === "undefined" || isMobile) {
    return;
  }
  const worker = new Worker(relHref + 'styles/hljs-worker.js');
  const snippets = $('pre code:not(.lang-mermaid)');
  snippets.each(function(i, block) {
    worker.onmessage = (event) => {
      const code = event.data.innerText;
      if (!code) {
        return;
      }
      const el = $(snippets.get(event.data.id));
      el.html(code).addClass("hljs");
    };
    const language = Object.values(block.classList).filter(el => el.startsWith("lang-")).map(el => el.substring(5)).pop();
    const innerText = block.textContent;
    worker.postMessage({id: i, innerText, language});
  });
}

function renderFlowcharts() {
  if (typeof mermaid === "undefined") {
    return;
  }
  const style = getComputedStyle(document.documentElement);
  mermaid.initialize({
    theme: "base",
    themeVariables: {
      primaryColor: style.getPropertyValue("--diagr-primary-color").slice(1),
      primaryBorderColor: style.getPropertyValue("--diagr-primary-border-color").slice(1),
      primaryTextColor: style.getPropertyValue("--diagr-primary-text-color").slice(1),
      secondaryColor: style.getPropertyValue("--diagr-secondary-color").slice(1),
      secondaryBorderColor: style.getPropertyValue("--diagr-secondary-border-color").slice(1),
      secondaryTextColor: style.getPropertyValue("--diagr-secondary-text-color").slice(1),
      tertiaryColor: style.getPropertyValue("--diagr-tertiary-color").slice(1),
      tertiaryBorderColor: style.getPropertyValue("--diagr-tertiary-border-color").slice(1),
      lineColor: style.getPropertyValue("--diagr-line-color").slice(1)
    },
    startOnLoad: false
  });
  mermaid.init(undefined, ".lang-mermaid");
}

function renderTabs() {
  const tabGroups = $(".tabGroupAlt");
  tabGroups.find("ul[role=\"tablist\"] > li > a").each(function() {
    const el = $(this);
    el.attr("href", '#');
    checkTabForCodeBlock(el);
    checkTabForActiveState(el);
  });
  tabGroups.find("ul[role=\"tablist\"] > li > a").on("click", function(ev) {
    const el = $(ev.target);
    const secs = el.closest(".tabGroupAlt").children("section[role=\"tabpanel\"]");
    const elItem = el.closest("ul").children("li");
    elItem.removeClass("active");
    elItem.children("a").eq(0).removeAttr("aria-selected");
    secs.attr("hidden", true).attr("aria-hidden", true);
    el.parent().toggleClass("active");
    secs.closest("[data-tab=\"" + el.attr("data-tab") + "\"]").eq(0).attr("hidden", false).attr("aria-hidden", false);
  });
}

function checkTabForCodeBlock(el) {
  const tabId = el.attr("data-tab");
  if (!tabId) {
    return;
  }
  const tabContent = el.closest(".tabGroupAlt").find("> section[data-tab=\"" + tabId + "\"]");
  if (tabContent.children().length != 1 || tabContent.children().find("code").length == 0) {
    return;
  }
  tabContent.addClass("code");
  el.parent().addClass("code");
}

function checkTabForActiveState(el) {
  if (el.attr("aria-selected") !== "true") {
    return;
  }
  el.parent().parent().children().removeClass("active");
  el.parent().addClass("active");
}

function toggleMenu() {
  const el = !this.classList.contains("blackout") ? $(this) : null;
  const x = $(".main-panel");
  const b = $(".blackout");
  el?.toggleClass("active");
  x.toggleClass("expand");
  b.toggleClass("active");
}

var HISTORY_SUPPORT = !!(history && history.pushState);

function scrollIfAnchor(link, pushToHistory) {
  if (!/^#[^ ]+$/.test(link)) {
    return false;
  }
  const match = document.getElementById(link.slice(1));
  if (!match) {
    return false;
  }
  $(".content-column").scrollTop(match.offsetTop);
  if (HISTORY_SUPPORT && pushToHistory) history.pushState({}, document.title, location.pathname + link);
  return true;
}

function delegateAnchors(ev) {
  if (!scrollIfAnchor(ev.target.getAttribute("href"), true)) {
    return;
  }
  ev.preventDefault();
}
