using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using src.Models;
using src.Services;
using src.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace src.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskService _taskService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskController(TaskService taskService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tasks = await _taskService.GetTasksAsync();

            return View(tasks.Select(x => _mapper.Map<TaskViewModel>(x)).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string taskId)
        {
            var task = await _taskService.GetTaskAsync(taskId);

            return View(_mapper.Map<TaskViewModel>(task));
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskModel task)
        {
            if (ModelState.IsValid)
            {
                await _taskService.AddTaskAsync(task);
                return RedirectToAction("Index");
            }

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskModel updatedTask)
        {
            if (ModelState.IsValid)
            {
                await _taskService.UpdateTaskAsync(updatedTask);
                return RedirectToAction("Index");
            }

            return View(_mapper.Map<TaskViewModel>(updatedTask));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string taskId)
        {
            await _taskService.DeleteTaskAsync(taskId);
            return RedirectToAction("Index");
        }
    }
}
