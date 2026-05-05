import api from './api';

export const getAllCourses = () => api.get('/course');
export const getCourseById = (id) => api.get(`/course/${id}`);
export const createCourse = (data) => api.post('/course', data);
export const updateCourse = (id, data) => api.put(`/course/${id}`, data);
export const deleteCourse = (id) => api.delete(`/course/${id}`);
export const assignInstructor = (courseId, instructorId) =>
  api.put(`/course/${courseId}/instructor/${instructorId}`);
