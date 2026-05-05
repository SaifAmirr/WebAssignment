import api from './api';

export const getAllInstructors = () => api.get('/instructor');
export const getInstructorById = (id) => api.get(`/instructor/${id}`);
export const createInstructor = (data) => api.post('/instructor', data);
export const updateInstructor = (id, data) => api.put(`/instructor/${id}`, data);
export const getInstructorCourses = (id) => api.get(`/instructor/${id}/courses`);
export const getInstructorProfile = (id) => api.get(`/instructor/${id}/profile`);
export const updateInstructorProfile = (id, data) => api.put(`/instructor/${id}/profile`, data);
