(function () {
  function updateClock() {
    var el = document.getElementById("topbarClock");
    if (!el) return;

    var now = new Date();
    el.textContent = now.toLocaleString("vi-VN", {
      weekday: "short",
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit"
    });
  }

  function setupSidebar() {
    var toggle = document.getElementById("sidebarToggle");
    var backdrop = document.getElementById("sidebarBackdrop");
    if (!toggle) return;

    toggle.addEventListener("click", function () {
      if (window.matchMedia("(max-width: 767.98px)").matches) {
        document.body.classList.toggle("sidebar-open");
      } else {
        document.body.classList.toggle("sidebar-collapsed");
      }
    });

    if (backdrop) {
      backdrop.addEventListener("click", function () {
        document.body.classList.remove("sidebar-open");
      });
    }
  }

  function setupClientFilters() {
    document.querySelectorAll("[data-filter-target]").forEach(function (input) {
      input.addEventListener("input", function () {
        var selector = input.getAttribute("data-filter-target");
        var query = input.value.trim().toLowerCase();
        document.querySelectorAll(selector).forEach(function (row) {
          var haystack = (row.getAttribute("data-search") || row.textContent || "").toLowerCase();
          row.hidden = query.length > 0 && haystack.indexOf(query) === -1;
        });
      });
    });

    document.querySelectorAll("[data-status-filter]").forEach(function (select) {
      select.addEventListener("change", function () {
        var selector = select.getAttribute("data-status-filter");
        var value = select.value;
        document.querySelectorAll(selector).forEach(function (row) {
          row.hidden = value.length > 0 && row.getAttribute("data-status") !== value;
        });
      });
    });
  }

  function setupTooltips() {
    if (!window.bootstrap) return;
    document.querySelectorAll("[data-bs-toggle='tooltip']").forEach(function (el) {
      new bootstrap.Tooltip(el);
    });
  }

  function setupToasts() {
    document.querySelectorAll(".toast-notice").forEach(function (toast) {
      window.setTimeout(function () {
        toast.style.opacity = "0";
        toast.style.transform = "translateY(-8px)";
      }, 3000);

      window.setTimeout(function () {
        toast.remove();
      }, 3400);
    });
  }

  document.addEventListener("DOMContentLoaded", function () {
    updateClock();
    window.setInterval(updateClock, 1000 * 30);
    setupSidebar();
    setupClientFilters();
    setupTooltips();
    setupToasts();
  });
})();
